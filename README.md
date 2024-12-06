# Santander HackerNews Assessment

## Running the Application
The program is specifically designed to avoid any infrastructure dependencies. It runs like any standard .NET project. I recommend opening the solution in Visual Studio and running the program in HTTP configuration.

 ## DEMO

 Here's short video demonstrating the application. As we can see We can edit beststories and it's being adjusted in real-time in our API - we can remove or add new beststories. We can also edit every single story.
 [https://youtu.be/4ppfm_gKC0Y](https://youtu.be/4ppfm_gKC0Y) It's demonstrated on my own Firebase db.

## Persistence

The application uses a simple NoSQL database, LiteDB. The use of LiteDB is limited to saving the list of news items to the database in real-time and restoring the list representation from the database into memory during application startup.  
This approach primarily prevents having an empty news list upon startup before news items are fetched from HackerNews.

## Core Assumptions and Program Structure Description
Essentially, if we want to handle a large number of requests without overloading the HackerNews API, we have three options:
1. Implement caching to store HackerNews data in memory and refresh the data periodically, e.g., every 5 minutes.
2. Do the same as in option 1 but use HTTP caching instead.
3. Store real-time data from HackerNews and react to its updates.
And... also a couple more that require additional infrastructure to cache like e.g. Redis

Given that the specified API uses real-time Firebase, I chose option 3.  
The application listens for updates to the list of top news stories via REST Streaming and reacts to changes in individual stories (e.g., stopping listening to a particular story and removing it from the database if it is no longer in `beststories.json`).

The `HackerNewsBackgroundService` is responsible for listening to Firebase. It primarily monitors the `beststories.json` list and manages the initiation and termination of listeners for individual stories as needed. In case the application stops, it ensures all connections are terminated properly.

I had some concerns regarding the number of (201) old connections to Firebase, but this did not cause any issues in the application nor according to Firebase's documentation, which supports up to 200,000 simultaneous connections.

Also it's important to mention that when we subscribe to each one of 200 stories we do it at rate no higher than 10 per second to avoid heavy load on API.

For better dependency separation, I used the `MediatR` library.

## Simplifications / Potential Development Points
Since the task did not specify an expected completion time, I was uncertain about the desired level of execution. The task could be completed in any timeframe ranging from one hour to several days.  
I believe the core part of the task is implemented correctly in a non-trivial way. Other aspects related to best practices would certainly be a plus, but I am unsure if task author expected me to focus on them.  
I hope that highlighting my awareness of these practices and noting that they should be included in a complete production-grade solution is sufficient.

Simplifications I made, which could be addressed with more time, include:
- Lack of project structure separating the presentation, application, and infrastructure layers.
- We could have better granulation of operations in our notification handlers
- Error handling and retry mechanisms using e.g. Polly
- Limited number of tests.
- No performance testing.
- Hardcoded values, such as the LiteDB file name in `StoriesRepository`.
- Missing Swagger configuration for accepted parameters and returned values.
- Inconsistent JSON serialization libraries: the controller uses `Text.JSON`, while Firebase event parsing uses `Newtonsoft`.
- (Far-reaching development) Preparing a Cloud-Native version.
- Preparing detailed documentation.
