﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Collections;
using OOC.Contract.Data.Response;
using OOC.Contract.Data.Common;
using OOC.Contract.Service;
using OOC.ORM;
using OOC.ServiceAttribute;
using System.Configuration;

namespace OOC.Service
{
    [ExposedService("FileService")]
    public class FileService : IFileService
    {
        private readonly string fileRoot = ConfigurationManager.AppSettings["fileRepositoryRoot"];

        public FileEntityResponse Get(string fileName)
        {
            string realPath = Path.Combine(new string[] { fileRoot, fileName }).ToString();
            if (!File.Exists(realPath) || !realPath.StartsWith(fileRoot))
            {
                return new FileEntityResponse(false, 1, "FILE_NOT_FOUND");
            }
            return new FileEntityResponse(fileName, File.ReadAllBytes(realPath));
        }

        public GenericResponse Put(string fileName, byte[] content)
        {
            string realPath = Path.Combine(new string[] { fileRoot, fileName }).ToString();
            if (!File.Exists(realPath) || !realPath.StartsWith(fileRoot))
            {
                return new FileEntityResponse(false, 1, "FILE_ALREADY_EXISTED");
            }
            File.WriteAllBytes(realPath, content);
            return new GenericResponse(true);
        }

        public FileListResponse List(string path)
        {
            string realPath = Path.Combine(new string[] { fileRoot, path }).ToString();
            if (!Directory.Exists(realPath) || !realPath.StartsWith(fileRoot))
            {
                return new FileListResponse(false, 1, "PATH_NOT_FOUND");
            }
            string[] files = Directory.GetFiles(realPath);
            List<FileDescription> descs = new List<FileDescription>();
            foreach (string file in files)
            {
                FileDescription desc = new FileDescription();
                desc.FileName = file.Substring(file.LastIndexOf("\\") + 1);
                desc.Size = (int)new FileInfo(file).Length;
                descs.Add(desc);
            }
            return new FileListResponse(descs.ToArray());
        }

        public GenericResponse Copy(string sourceFileName, string destFileName)
        {
            string srcRealPath = Path.Combine(new string[] { fileRoot, sourceFileName }).ToString();
            string dstRealPath = Path.Combine(new string[] { fileRoot, destFileName }).ToString();
            if (!File.Exists(srcRealPath) || !srcRealPath.StartsWith(fileRoot))
            {
                return new GenericResponse(false, 1, "SRC_FILE_NOT_FOUND");
            }
            if (File.Exists(dstRealPath) || !dstRealPath.StartsWith(fileRoot))
            {
                return new GenericResponse(false, 1, "DST_FILE_ALREADY_EXISTED");
            }
            File.Copy(srcRealPath, dstRealPath);
            return new GenericResponse(true);
        }

        public FileStatResponse Stat(string fileName)
        {
            string realPath = Path.Combine(new string[] { fileRoot, fileName }).ToString();
            if (!File.Exists(realPath) || !realPath.StartsWith(fileRoot))
            {
                return new FileStatResponse(false, 1, "FILE_NOT_FOUND");
            }
            FileInfo info = new FileInfo(realPath);
            return new FileStatResponse(fileName, info.Length);
        }

        public GenericResponse Delete(string fileName)
        {
            string realPath = Path.Combine(new string[] { fileRoot, fileName }).ToString();
            if (!realPath.StartsWith(fileRoot))
            {
                return new GenericResponse(false, 1, "ACCESS_DENIED");
            }
            if (Directory.Exists(realPath))
            {
                Directory.Delete(realPath, true);
                return new GenericResponse(true);
            }
            if (File.Exists(realPath))
            {
                File.Delete(realPath);
                return new GenericResponse(true);
            }
            return new GenericResponse(false, 1, "FILE_NOT_EXIST");
        }

        public GenericResponse CreateDirectory(string path)
        {
            string realPath = Path.Combine(new string[] { fileRoot, path }).ToString();
            if (!realPath.StartsWith(fileRoot))
            {
                return new GenericResponse(false, 1, "ACCESS_DENIED");
            }
            Directory.CreateDirectory(realPath);
            return new GenericResponse(true);
        }
    }
}
