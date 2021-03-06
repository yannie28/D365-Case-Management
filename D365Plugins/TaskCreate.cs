﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

// This plugin creates a Task record when user creates a Contact record. This task due date is within 2 days.

namespace D365Plugins
{
    public class TaskCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity contact = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Plug-in business logic goes here.
                    // Note: to check where to find the Logical Name, go to Customize the System > Fields of the Entity > use the 'Name' column
                    // 'Name' is also called Logical Name because it also indicates the column name in the DB
                    // 'Schema Name' is for Web API
                    // 'Display Name' is the one you can see in the UI

                    Entity taskRecord = new Entity("task");

                    // Single Line of Text
                    taskRecord.Attributes.Add("subject", "Follow Up");
                    taskRecord.Attributes.Add("description", "Please follow up with contact");

                    // Date
                    taskRecord.Attributes.Add("scheduledend", DateTime.Now.AddDays(2));

                    // Option Set
                    taskRecord.Attributes.Add("prioritycode", new OptionSetValue(2));

                    // ParentRecord or Lookup
                    // taskRecord.Attributes.Add("regardingobjectid", new EntityReference("contact", contact.Id));
                    taskRecord.Attributes.Add("regardingobjectid", contact.ToEntityReference()); // this is better because you don't have to create in memory object

                    Guid taskGuid = service.Create(taskRecord);

                 }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }

    }
}
