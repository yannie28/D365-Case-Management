using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

// This plugins prevents users from creating Contact record with duplicate email address

namespace D365Plugins
{
    public class DuplicateCheck : IPlugin
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

                    string email = String.Empty;

                    if (contact.Attributes.Contains("emailaddress1"))
                    {
                        email = contact.Attributes["emailaddress1"].ToString();

                        // select * from contact where emailaddress1 = 'email'

                        QueryExpression query = new QueryExpression("contact");
                        //query.ColumnSet = new ColumnSet(true) // retrieves all columns
                        query.ColumnSet = new ColumnSet(new string[] { "emailaddress1" }); // retrieves specific column names
                        query.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, email); // adds the where clause of the select statement


                    }

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
