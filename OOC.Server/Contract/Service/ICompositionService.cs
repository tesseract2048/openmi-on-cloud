﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using OOC.Contract.Data.Common;
using OOC.Contract.Data.Request;
using OOC.Contract.Data.Response;
using OOC.Entity;

namespace OOC.Contract.Service
{
    [ServiceContract]
    public interface ICompositionService
    {
        [OperationContract]
        List<Composition> QueryCompositionByAuthorUserId(int authorUserId);

        [OperationContract]
        List<Composition> QueryCompositionByKeyword(string keyword);

        [OperationContract]
        List<Composition> QueryCompositionByModel(string modelGuid);

        [OperationContract]
        string Create(int authorUserId, string title, bool isShared, bool isFinished);

        [OperationContract]
        List<CompositionModel> GetCompositionModels(string compositionGuid);

        [OperationContract]
        List<CompositionModelData> GetCompositionModelsData(string compositionGuid);

        [OperationContract]
        string CreateCompositionModel(string compositionGuid, string modelGuid, CompositionModelProperties properties);

        [OperationContract]
        void UpdateCompositionModelProperties(string cmGuid, CompositionModelProperties properties);

        [OperationContract]
        void UpdateCompositionModelProperty(string cmGuid, string key, string value);

        [OperationContract]
        CompositionModel GetCompositionModel(string cmGuid);

        [OperationContract]
        CompositionModelData GetCompositionModelData(string cmGuid);

        [OperationContract]
        void RemoveModel(string cmGuid);

        [OperationContract]
        List<CompositionLink> GetCompositionLinks(string compositionGuid);

        [OperationContract]
        List<CompositionLinkData> GetCompositionLinksData(string compositionGuid);

        [OperationContract]
        string CreateCompositionLink(string compositionGuid, string sourceCmGuid, string targetCmGuid, string sourceQuantity, string targetQuantity, string sourceElementSet, string targetElementSet, LinkDataOperation dataOperation);

        [OperationContract]
        CompositionLink GetCompositionLink(string linkGuid);

        [OperationContract]
        CompositionLinkData GetCompositionLinkData(string linkGuid);

        [OperationContract]
        void RemoveCompositionLink(string linkGuid);

        [OperationContract]
        CompositionData GetCompositionData(string compositionGuid);

        [OperationContract]
        string GenerateInputFileName(string compositionGuid, string modelGuid, string relativePath);

        [OperationContract]
        List<string> GetInputFileNames(string compositionGuid);

        [OperationContract]
        void UpdateCompositionTitle(string compositionGuid, string title);
    }
}
