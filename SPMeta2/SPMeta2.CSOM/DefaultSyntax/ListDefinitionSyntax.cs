﻿using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using SPMeta2.Models;
using SPMeta2.Syntax.Default;
using System;

namespace SPMeta2.CSOM.DefaultSyntax
{
    public static class ListDefinitionSyntax
    {
        #region methods

        #region behavior support

        //public static DefinitionBase OnCreating(this DefinitionBase model, Action<ListDefinition, SPList> action)
        //{
        //    model.RegisterModelUpdatingEvent(action);

        //    return model;
        //}

        //public static DefinitionBase OnCreated(this DefinitionBase model, Action<ListDefinition, SPList> action)
        //{
        //    model.RegisterModelUpdatedEvent(action);

        //    return model;
        //}

        #endregion

        #region add content type

        public static ModelNode AddContentTypeLink(this ModelNode model, ContentTypeId contentTypeId)
        {
            return ContentTypeLinkDefinitionSyntax.AddContentTypeLink(model, new ContentTypeLinkDefinition
            {
                ContentTypeId = contentTypeId.ToString()
            });
        }

        #endregion

        #endregion

        public static ModelNode OnCreating(this ModelNode model, Action<ListDefinition, List> action)
        {
            model.RegisterModelEvent<ListDefinition, List>(SPMeta2.Common.ModelEventType.OnUpdating, action);

            return model;
        }

        public static ModelNode OnCreated(this ModelNode model, Action<ListDefinition, List> action)
        {
            model.RegisterModelEvent<ListDefinition, List>(SPMeta2.Common.ModelEventType.OnUpdated, action);

            return model;
        }


        #region utils

        public static string GetListUrl(this ListDefinition listDefinition)
        {
            // BIG BIG BIG TODO
            // correct list/doc lib mapping has to be implemented and tested
            // very critical part of the whole provision lib

            var templateType = (ListTemplateType)listDefinition.TemplateType;

            switch (templateType)
            {
                case ListTemplateType.Events:
                case ListTemplateType.Tasks:
                case ListTemplateType.GenericList:
                case ListTemplateType.AdminTasks:
                case ListTemplateType.Agenda:
                case ListTemplateType.Announcements:
                case ListTemplateType.Comments:
                case ListTemplateType.Contacts:
                case ListTemplateType.DiscussionBoard:
                case ListTemplateType.ExternalList:
                case ListTemplateType.Links:

                    return "Lists/" + listDefinition.Url;

                case ListTemplateType.CallTrack:
                case ListTemplateType.Categories:
                case ListTemplateType.Circulation:
                case ListTemplateType.CustomGrid:
                case ListTemplateType.DataConnectionLibrary:
                case ListTemplateType.DataSources:
                case ListTemplateType.Decision:
                case ListTemplateType.DocumentLibrary:

                case ListTemplateType.Facility:
                case ListTemplateType.GanttTasks:
                case ListTemplateType.HealthReports:
                case ListTemplateType.HealthRules:
                case ListTemplateType.Holidays:
                case ListTemplateType.HomePageLibrary:
                case ListTemplateType.IMEDic:
                case ListTemplateType.InvalidType:
                case ListTemplateType.IssueTracking:

                case ListTemplateType.ListTemplateCatalog:
                case ListTemplateType.MasterPageCatalog:
                case ListTemplateType.MeetingObjective:
                case ListTemplateType.MeetingUser:
                case ListTemplateType.Meetings:
                case ListTemplateType.NoCodePublic:
                case ListTemplateType.NoCodeWorkflows:
                case ListTemplateType.NoListTemplate:
                case ListTemplateType.PictureLibrary:
                case ListTemplateType.Posts:
                case ListTemplateType.SolutionCatalog:
                case ListTemplateType.Survey:
                case ListTemplateType.TextBox:
                case ListTemplateType.ThemeCatalog:
                case ListTemplateType.ThingsToBring:
                case ListTemplateType.Timecard:
                case ListTemplateType.UserInformation:
                case ListTemplateType.WebPageLibrary:
                case ListTemplateType.WebPartCatalog:
                case ListTemplateType.WebTemplateCatalog:
                case ListTemplateType.Whereabouts:
                case ListTemplateType.WorkflowHistory:
                case ListTemplateType.WorkflowProcess:
                case ListTemplateType.XMLForm:

                    break;
                default:
                    break;
            }

            return listDefinition.Url;
        }


        #endregion
    }
}
