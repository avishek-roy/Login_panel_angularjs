
USAGE:

1. Make a configuration file like the example below and add your configuration

2. Instantiate a new ChangeTrackingService and subscribe to its OnChangeTracked event. In this eventhandler you can add logic to process tracked changes. 
	The eventargs will hold a changeset containing the changes.

3. (Optional) If you want to track additional custom data, supply the ChangeTrackingService object with an implementation of ICustomDataProvider.

4. Wrap the changing of the configured domain objects in a using clause like this:

	using (changeTrackingService.BeginTracking(objectToBeTracked))
	{
		//do some changes to objectToBeTracked
	}






EXAMPLE CONFIGURATION:


public static class ChangeTrackingConfigurator
    {
        public static void Configure()
        {
            ChangeTrackingConfigContainer.Config.ConfigureTracking(config =>
                {
                    //Example configuration:
                    //config.For<YourTypeToBeTracked>()
                    //      .TrackProperty(x => x.Property).As("DisplayName").Using(x => x.PropertyMethod)
                    //      .TrackProperty(x => x.Property).WithSerializer(new PropertySpecificSerializer());
                });
        }
    }
