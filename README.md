## Btw, you can hire the author of SPMeta2!
My name is <a href="http://www.linkedin.com/in/antonvishnyakov">Anton Vishnyakov</a> and I am looking for an awesome place to work in sunny Sydney, Australia. I have <a href="http://www.linkedin.com/in/antonvishnyakov">an awesome SharePoint and custom development skillset (check LinkedIn)</a> , and I have 457 visa.

Currently, I am in Sydney, Australia. I have been for the last two years in Sydney as a consultant across various big and small clients in Sydney (20+ in total), for both in-house and client faces engagements; user training, custom development, infrastructure engagements, bug-fixing and pretty much everything related to SharePoint and development.

Look up me at LinkedIn: <a href="http://www.linkedin.com/in/antonvishnyakov">http://www.linkedin.com/in/antonvishnyakov</a> or check more details here - <a href="http://spdevlab.com/2014/06/25/will-code-sharepoint-for-food-in-sydney-australia/">Will code SharePoint for food and donuts in Sydney, Australia</a>


## What is SPMeta2?
SPMeta2 is a fluent API for code-based SharePoint artifact provisioning.

Struggling with SharePoint's API inconsistency, bugs, "by-design" behaviour, unaffordable amount of time to write, support and upgrade WSP packages and XML, a team of passionate SharePoint professionals decided to come up with robust, testable and repeatable way to deploy such artifacts like fields, content types, libraries, pages and many more.

As an outcome, we created SPMeta2 - a .NET 4.5 library to provide fluent API for SharePoint 2013 artifact with SSOM/CSOM or JSOM for both on premise and O365 instances. What's inside?

## SPMeta2 philosophy and mission
### Fluent API and syntax extensions
SPMeta2 API allows you to define SharePoint artifacts such as field, content type, list (and many more), define relationships between them and, finally, deploy them via SSOM/CSOM. You work with c# POCO objects defining your data model, we take care about the rest. 

SPMeta2 might be extended with custom syntax implementation to meet your project needs. Extension methods are used to adjust a specific behavior or property of SharePoint artifacts. Custom syntax or DSL can easily be created to address specific project needs.

Learn more here - <a href="https://github.com/SubPointSolutions/spmeta2/wiki/How-SPMeta2-Works">How SPMeta2 Works</a>.

### Model tree build-in validation
The model tree might be optionally validated with build in rules. It guarantees nobody adds a field to the web or list to the site collection. Custom validators can be implemented to address your project needs as well. 

Builtin validators ensure SharePoint limitations and boundaries. SPMeta2 checks that internal names of all your fields don not exceed 32 chars. There is more magic than you can imagine.

Learn more here - <a href="https://github.com/SubPointSolutions/spmeta2/wiki/SPMeta2.Validation-package">SPMeta2.Validation package </a>.

### SharePoint 2013 Foundation, Standard, Enterprise and O365 are supported
SPMeta2 supports all SharePoint editions. It is splitted up into several packages to reflect SharePoint editions: Foundation, Standart and Enterprise. O365 support is implemented with CSOM.

### SSOM and CSOM are supported. JSOM is coming up.
SPMeta2 supports SSOM and CSOM. We are working on JSOM support implemeted with TypeScript and SPTypeScript.

### No XML inside - only code
SPMeta2 is code based provision library for SharePoint 2013.
* You DO NOT write XML
* You DO NOT write WSP
* You DO write code instead

Learn more here - <a href="https://github.com/SubPointSolutions/spmeta2/wiki/How-SPMeta2-Works">How SPMeta2 Works</a>.

### Regression and testing are implemented and supported
Having code based provision allows us to have full control over the provision and update flow. As there is no WSP or XML, not features need to be deployed or activated. 

This allows us to write integration tests within minutes, make sure deployment and upgrade work as expected. Most of the provision cases are covered with integration tests. We create a new site or web, deploy everything we need and check if everything has been deployed correctly.

SPMeta2 uses [appveyor CI online service](http://www.appveyor.com/) at https://ci.appveyor.com/project/avishnyakov/spmeta2
Current build status for the master branch: ![](https://ci.appveyor.com/api/projects/status/i96tsrq5xjdm4tu2)

Learn more here - <a href="https://github.com/SubPointSolutions/spmeta2/wiki/Regression-and-CI">Regression and CI</a>.


## Documentation and support
<ul>
                    <li><a target="_blank" href="https://github.com/SubPointSolutions/spmeta2">SPMeta2 @ GitHub</a></li>
                    <li><a target="_blank" href="https://www.nuget.org/packages?q=spmeta2">SPMeta2 @ Nuget</a></li>
                    <li><a target="_blank" href="https://github.com/SubPointSolutions/spmeta2/wiki">SPMeta2 Documentation Wiki</a></li>
                    <li><a target="_blank" href="https://github.com/SubPointSolutions/spmeta2/issues">SPMeta2 Bugtracker</a></li>
                    <li><a target="_blank" href="http://subpointsolutions.github.io/spmeta2/Help">SPMeta2 API Documentation</a></li>
                </ul>
