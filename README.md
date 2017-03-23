# campspot-GetAvailableCampsites

## a. How to build and run your program and tests
Because I developed this program using .NET, my solution can only be run on Windows. You also need to install Visual Studio Community 2015+ to be able to use the programs required.

First, [download the releases](https://github.com/logeyG/campspot-GetAvailableCampsites/releases/tag/1.0) for the test project and the main program. Extract these folders to a local directory, hereby referred to as `~`.

### To run the automated tests
1. Open up the Developer Command Prompt for Visual Studio which is installed with the Visual Studio installation
2. Navigate to `C:\Program Files (x86)\Microsoft Visual Studio [VS Version #]\Common7\IDE\CommonExtensions\Microsoft\TestWindow`
3. run `Vstest.console.exe ~\GetAvailableCampsites.Test.Release\Release\GetAvailableCampsites.Test.dll`

### To run the program
1. Using any command prompt, navigate to `~\GetAvailableCampsites.Release\Release`
2. run `GetAvailableCampsites.exe [.json file]`

## b. A high-level description of your approach to solving the problem
My solution was designed specifically to be used as a web service. I have included all the models within the request for ease of use and code readability. The main method that is called is the following:
```C#
List<Campsite> GetAvailableCampsites(GetAvailableCampsitesRequest request)
```

As you can see, the method accepts a request that is an object representation of the input .json file, and returns a list of available campsites.

First, before we begin checking each campsite for validity, we can remove campsites that currently have a reservation during the time period we are searching for. This is done by calling the following method:
```c#
List<Campsite> RemoveUnavailableCampsites(GetAvailableCampsitesRequest request)
```

Now that we have our campsite list of *possible* campsites, we can begin adding our reservation and checking if that reservation is valid or not. After the reservation is added, the following method is called:
```c#
bool HasValidReservations(List<Reservation> reservations, List<GapRule> gapRules)
```
This method looks at the gaps *adjaent to the new reservation* on that campsite, and checks against the gap rules. If all the gaps don't conflict with any of the gap rules, then this campsite is available for our searched reservation.

After we have all the available campsites, we simply return this list.

## c. Any assumptions or special considerations that were made
As is shown from the name of the method calls and the general architecture of the program, I paid special attention in designing this program so it could be easily adapted to a web service call. Because of this, I do perform some validation on the request, but I do assume that there would be some client side validation taking place.
