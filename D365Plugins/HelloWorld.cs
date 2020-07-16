using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace D365Plugins
{
    public class HelloWorld : IPlugin
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
                Entity entity = (Entity)context.InputParameters["Target"];

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

                    // Read form attribute values
                    string firstName = string.Empty;

                    if (entity.Attributes.Contains("firstname"))
                    {
                        firstName = entity.Attributes["firstname"].ToString(); // Get firstName of the entity
                    };

                    string lastName = entity.Attributes["lastname"].ToString(); // Get lastName of the entity

                    // Assign value to form attribute
                    // Intercepting and updating attribute collection of primary entity which is being passed to the main event
                    entity.Attributes.Add("description", "Hello World " + firstName + " " + lastName);
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
