﻿using System.Linq;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPMeta2.Definitions;
using SPMeta2.Regression.Common.Utils;
using SPMeta2.SSOM.ModelHandlers;
using SPMeta2.Utils;

namespace SPMeta2.Regression.SSOM.Validation
{
    public class SecurityGroupLinkDefinitionValidator : SecurityGroupLinkModelHandler
    {
        #region methods

        protected override void DeployModelInternal(object modelHost, DefinitionBase model)
        {
            var securableObject = modelHost.WithAssertAndCast<SPSecurableObject>("modelHost", value => value.RequireNotNull());
            var securityGroupLinkModel = model.WithAssertAndCast<SecurityGroupLinkDefinition>("model", value => value.RequireNotNull());

            var web = GetWebFromSPSecurableObject(securableObject);
            var securityGroup = web.SiteGroups[securityGroupLinkModel.SecurityGroupName];
            //securableObject.RoleAssignments

            TraceUtils.WithScope(traceScope =>
            {
                traceScope.WriteLine(string.Format("Validate model:[{0}] securableObject:[{1}]", securityGroupLinkModel, securityGroup));

                traceScope.WithTraceIndent(trace =>
                {
                    // asserting it exists
                    trace.WriteLine(string.Format("Validating existance..."));

                    Assert.IsTrue(securableObject
                                            .RoleAssignments
                                            .OfType<SPRoleAssignment>()
                                            .FirstOrDefault(a => a.Member.ID == securityGroup.ID) != null);

                    trace.WriteLine(string.Format("RoleAssignments for security group link [{0}] exists.", securityGroupLinkModel.SecurityGroupName));
                });
            });
        }

        #endregion
    }
}
