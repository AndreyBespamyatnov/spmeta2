﻿using System;
using Microsoft.SharePoint.Client;
using SPMeta2.Common;
using SPMeta2.CSOM.Extensions;
using SPMeta2.Definitions;
using SPMeta2.ModelHandlers;
using SPMeta2.Utils;

namespace SPMeta2.CSOM.ModelHandlers
{
    public class ContentTypeLinkModelHandler : CSOMModelHandlerBase
    {
        public override Type TargetType
        {
            get { return typeof(ContentTypeLinkDefinition); }
        }

        protected override void DeployModelInternal(object modelHost, DefinitionBase model)
        {
            var list = modelHost.WithAssertAndCast<List>("modelHost", value => value.RequireNotNull());
            var contentTypeLinkModel = model.WithAssertAndCast<ContentTypeLinkDefinition>("model", value => value.RequireNotNull());

            var context = list.Context;

            context.Load(list, l => l.ContentTypesEnabled);
            context.ExecuteQuery();

            if (list.ContentTypesEnabled)
            {
                var web = list.ParentWeb;

                context.Load(web, w => w.AvailableContentTypes);
                context.Load(list, l => l.ContentTypes);

                context.ExecuteQuery();

                var targetContentType = FindSiteContentType(web, contentTypeLinkModel);
                var listContentType = FindListContentType(list, contentTypeLinkModel);

                InvokeOnModelEvent(this, new ModelEventArgs
                {
                    CurrentModelNode = null,
                    Model = null,
                    EventType = ModelEventType.OnProvisioning,
                    Object = listContentType,
                    ObjectType = typeof(ContentType),
                    ObjectDefinition = model,
                    ModelHost = modelHost
                });

                if (targetContentType != null && listContentType == null)
                {
                    var ct = list.ContentTypes.Add(new ContentTypeCreationInformation
                    {
                        Description = targetContentType.Description,
                        Group = targetContentType.Group,
                        Name = targetContentType.Name,
                        ParentContentType = targetContentType
                    });

                    InvokeOnModelEvent(this, new ModelEventArgs
                    {
                        CurrentModelNode = null,
                        Model = null,
                        EventType = ModelEventType.OnProvisioned,
                        Object = ct,
                        ObjectType = typeof(ContentType),
                        ObjectDefinition = model,
                        ModelHost = modelHost
                    });

                    list.Update();
                    context.ExecuteQuery();
                }
                else
                {
                    InvokeOnModelEvent(this, new ModelEventArgs
                    {
                        CurrentModelNode = null,
                        Model = null,
                        EventType = ModelEventType.OnProvisioned,
                        Object = listContentType,
                        ObjectType = typeof(ContentType),
                        ObjectDefinition = model,
                        ModelHost = modelHost
                    });

                    listContentType.Update(false);
                    context.ExecuteQuery();
                }
            }
        }

        protected ContentType FindListContentType(List list, ContentTypeLinkDefinition contentTypeLinkModel)
        {
            // TODO
            // https://github.com/SubPointSolutions/spmeta2/issues/68

            // if content type name was not provided, this fails
            // should be re-done by ID and Name
            // OOTB content types could be binded by ID, and custom content types might be binded by name

            if (!string.IsNullOrEmpty(contentTypeLinkModel.ContentTypeName))
                return list.ContentTypes.FindByName(contentTypeLinkModel.ContentTypeName);

            if (!string.IsNullOrEmpty(contentTypeLinkModel.ContentTypeId))
                return list.ContentTypes.GetById(contentTypeLinkModel.ContentTypeId);

            throw new Exception(
                string.Format("Either ContentTypeName or ContentTypeId must be provides. Can't lookup current list content type by Name:[{0}] and ContentTypeId:[{1}] provided.",
                contentTypeLinkModel.ContentTypeName, contentTypeLinkModel.ContentTypeId));
        }

        protected ContentType FindSiteContentType(Web web, ContentTypeLinkDefinition contentTypeLinkModel)
        {
            ContentType targetContentType = null;

            if (!string.IsNullOrEmpty(contentTypeLinkModel.ContentTypeName))
                targetContentType = web.AvailableContentTypes.FindByName(contentTypeLinkModel.ContentTypeName);

            if (targetContentType == null && !string.IsNullOrEmpty(contentTypeLinkModel.ContentTypeId))
                targetContentType = web.AvailableContentTypes.FindById(contentTypeLinkModel.ContentTypeId);

            if (targetContentType == null)
                throw new Exception(string.Format("Cannot find content type specified by model: id:[{0}] name:[{1}]",
                                            contentTypeLinkModel.ContentTypeId, contentTypeLinkModel.ContentTypeName));

            return targetContentType;
        }
    }
}
