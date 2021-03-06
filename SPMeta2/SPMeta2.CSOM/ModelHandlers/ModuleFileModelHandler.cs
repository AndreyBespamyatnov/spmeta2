﻿using System;
using Microsoft.SharePoint.Client;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.Definitions;
using SPMeta2.ModelHandlers;
using SPMeta2.Utils;
using SPMeta2.Common;

namespace SPMeta2.CSOM.ModelHandlers
{
    public class ModuleFileModelHandler : CSOMModelHandlerBase
    {
        #region properties

        public override Type TargetType
        {
            get { return typeof(ModuleFileDefinition); }
        }

        #endregion

        #region methods

        public override void DeployModel(object modelHost, DefinitionBase model)
        {
            var folderHost = modelHost.WithAssertAndCast<FolderModelHost>("modelHost", value => value.RequireNotNull());
            var moduleFile = model.WithAssertAndCast<ModuleFileDefinition>("model", value => value.RequireNotNull());

            ProcessFile(folderHost, moduleFile);
        }

        private string GetSafeFileUrl(Folder folder, ModuleFileDefinition moduleFile)
        {
            return folder.ServerRelativeUrl + "/" + moduleFile.FileName;
        }

        public override void WithResolvingModelHost(object modelHost, DefinitionBase model, Type childModelType, Action<object> action)
        {
            var folderHost = modelHost.WithAssertAndCast<FolderModelHost>("modelHost", value => value.RequireNotNull());
            var moduleFile = model.WithAssertAndCast<ModuleFileDefinition>("model", value => value.RequireNotNull());

            var web = folderHost.CurrentWeb;
            var file = ProcessFile(folderHost, moduleFile);
            var context = file.Context;

            if (childModelType == typeof(ListItemFieldValueDefinition))
            {
                var fileListItem = file.ListItemAllFields;

                context.Load(fileListItem, i => i.Id, i => i.ParentList);
                context.ExecuteQuery();

                var list = fileListItem.ParentList;
                var item = list.GetItemById(fileListItem.Id);

                context.ExecuteQuery();

                var listItemPropertyHost = new ListItemFieldValueModelHost
                {
                    CurrentItem = item
                };

                action(listItemPropertyHost);

                item.Update();

                context.ExecuteQuery();
            }
            else if (childModelType == typeof(PropertyDefinition))
            {
                var propModelHost = new PropertyModelHost
                {
                    CurrentFile = file
                };

                action(propModelHost);

                context.ExecuteQuery();
            }
            else
            {
                action(file);

                context.ExecuteQuery();
            }
        }

        //private File GetFile()
        //{

        //}

        private void WithSafeFileOperation(List list, File file, Func<File, File> action)
        {
            var context = file.Context;

            context.Load(file, f => f.Exists);
            context.ExecuteQuery();

            if (file.Exists)
            {
                context.Load(file, f => f.CheckOutType);
                context.Load(file, f => f.Level);

                context.ExecuteQuery();
            }


            // big todo with correct update and punblishing
            // get prev SPMeta2 impl for publishing pages
            if (list != null && (file.Exists && file.CheckOutType != CheckOutType.None))
                file.UndoCheckOut();

            if (list != null && (list.EnableMinorVersions || list.EnableVersioning) && (file.Exists && file.Level == FileLevel.Published))
                file.UnPublish("Provision");

            if (list != null && (file.Exists && file.CheckOutType == CheckOutType.None))
                file.CheckOut();

            context.ExecuteQuery();

            var spFile = action(file);

            context.ExecuteQuery();

            context.Load(spFile, f => f.Exists);
            context.ExecuteQuery();

            if (spFile.Exists)
            {
                context.Load(spFile, f => f.CheckOutType);
                context.Load(spFile, f => f.Level);

                context.ExecuteQuery();
            }

            if (list != null && (spFile.Exists && spFile.CheckOutType != CheckOutType.None))
                spFile.CheckIn("", CheckinType.MajorCheckIn);

            if (list != null && (list.EnableMinorVersions || list.EnableVersioning))
                spFile.Publish("Provision");

            if (list != null && (list.EnableModeration))
                spFile.Approve("Provision");

            context.ExecuteQuery();
        }

        private File ProcessFile(FolderModelHost folderHost, ModuleFileDefinition moduleFile)
        {
            var context = folderHost.CurrentLibraryFolder.Context;

            var web = folderHost.CurrentWeb;
            var list = folderHost.CurrentList;
            var folder = folderHost.CurrentLibraryFolder;

            context.Load(folder, f => f.ServerRelativeUrl);
            context.ExecuteQuery();

            if (list != null)
            {
                context.Load(list, l => l.EnableMinorVersions);
                context.Load(list, l => l.EnableVersioning);
                context.Load(list, l => l.EnableModeration);

                context.ExecuteQuery();
            }

            var file = web.GetFileByServerRelativeUrl(GetSafeFileUrl(folder, moduleFile));

            context.Load(file, f => f.Exists);
            context.ExecuteQuery();

            InvokeOnModelEvent<ModuleFileDefinition, File>(file, ModelEventType.OnUpdating);

            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioning,
                Object = file.Exists ? file : null,
                ObjectType = typeof(File),
                ObjectDefinition = moduleFile,
                ModelHost = folderHost
            });


            WithSafeFileOperation(list, file, f =>
            {
                var fileName = moduleFile.FileName;
                var fileContent = moduleFile.Content;

                var fileCreatingInfo = new FileCreationInformation
                {
                    Url = fileName,
                    Content = fileContent,
                    Overwrite = file.Exists
                };

                return folder.Files.Add(fileCreatingInfo);

            });

            var resultFile = web.GetFileByServerRelativeUrl(GetSafeFileUrl(folder, moduleFile));

            context.Load(resultFile, f => f.Exists);
            context.ExecuteQuery();

            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioned,
                Object = resultFile,
                ObjectType = typeof(File),
                ObjectDefinition = moduleFile,
                ModelHost = folderHost
            });
            InvokeOnModelEvent<ModuleFileDefinition, File>(resultFile, ModelEventType.OnUpdated);

            return resultFile;
        }

        #endregion
    }
}
