﻿using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;
using OOC.Entity;
using OOC.Contract.Data.Common;
using OOC.Contract.Data.Request;
using OOC.Contract.Data.Response;
using OOC.Contract.Service;
using OOC.ServiceAttribute;
using OOC.Util;

namespace OOC.Service
{
    [ExposedService("TaskService")]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TaskService : ITaskService
    {
        private static readonly object assigningLock = new object();

        public string Create(string compositionGuid, int userId)
        {
            using (OOCEntities db = new OOCEntities())
            {
                IQueryable<Composition> result = from o in db.Composition
                                                 where o.guid == compositionGuid
                                                 select o;
                if (!result.Any())
                {
                    throw new FaultException("COMPOSITION_NOT_EXISTS");
                }
                Composition composition = result.First();
                CompositionData compositionData = new CompositionData(composition);
                Task task = new Task()
                {
                    guid = GuidUtil.newGuid(),
                    compositionGuid = compositionGuid,
                    compositionData = compositionData.Serialized,
                    state = (sbyte)TaskState.Created,
                    userId = userId,
                    modelProgress = new ModelProgress().Serialized
                };
                try
                {
                    db.Task.AddObject(task);
                    db.SaveChanges();
                }
                catch
                {
                    throw new FaultException("TRANSACTION_FAILED");
                }
                return task.guid;
            }
        }

        public void UpdateState(string guid, TaskState state)
        {
            using (OOCEntities db = new OOCEntities())
            {
                IQueryable<Task> result = from o in db.Task
                                          where o.guid == guid
                                          select o;
                if (!result.Any())
                {
                    throw new FaultException("TASK_NOT_EXISTS");
                }
                result.First().state = (sbyte)state;
                db.SaveChanges();
            }
        }

        public TaskAssignResponse AssignPendingTask(string instanceName)
        {
            lock (assigningLock)
            {
                using (OOCEntities db = new OOCEntities())
                {
                    IQueryable<Task> result = from o in db.Task
                                              where o.state == (sbyte)TaskState.Ready
                                              select o;
                    if (!result.Any())
                    {
                        throw new FaultException("NO_TASK_AVAILABLE");
                    }
                    Task task = result.First();
                    task.state = (sbyte)TaskState.Assigned;
                    task.instanceName = instanceName;
                    db.SaveChanges();
                    return new TaskAssignResponse(task, QueryTaskFileMapping(task.guid, TaskFileType.Input));
                }
            }
        }

        public TaskInfoResponse QueryTaskByGuid(string guid)
        {
            using (OOCEntities db = new OOCEntities())
            {
                IQueryable<Task> result = from o in db.Task
                                          where o.guid == guid
                                          select o;
                if (!result.Any())
                {
                    throw new FaultException("TASK_NOT_EXISTS");
                }
                return new TaskInfoResponse(result.First());
            }
        }


        public void AddTaskFileMapping(string guid, string fileName, string relativePath, TaskFileType type, bool isDownloadable)
        {
            using (OOCEntities db = new OOCEntities())
            {
                TaskFileMapping taskFileMapping = new TaskFileMapping()
                {
                    taskGuid = guid,
                    fileName = fileName,
                    relativePath = relativePath,
                    type = (sbyte)type,
                    isDownloadable = isDownloadable
                };
                db.TaskFileMapping.AddObject(taskFileMapping);
                db.SaveChanges();
            }
        }

        public List<TaskFileMapping> QueryTaskFileMapping(string guid, TaskFileType type)
        {
            using (OOCEntities db = new OOCEntities())
            {
                IQueryable<TaskFileMapping> result = from o in db.TaskFileMapping
                                                     where o.taskGuid == guid && o.type == (sbyte)type
                                                     select o;
                return result.ToList();
            }
        }

        public string GenerateTaskFileName(string guid, TaskFileType type, string relativePath)
        {
            return @"Tasks\" + guid + @"\" + type + @"\" + relativePath;
        }
    }
}
