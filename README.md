# Drivers-Booking-School
Resources - C#, .Net Framework, Microsoft SQL Client Server

This is a Drivers School administration system that I created as a project to further remind myself of the content I was taught in my undergrad in the University of Kwa-Zulu Natal.

The system has several pages, such as LogIn, Start, View_Bookings, and so forth.
The pages are all connected together.
By default, if you create a .Net Framework project, you're given a Program class that is practically the main class that runs the entire program.
I further created a Connect class that will handle all the processed that require a database connection, such as updating the client details, or checking if there is a double booking.

If you have Visual Studio 2015 and above, you can download the code, open it by clicking the .sln file, and then run it.
Don't worry about log in details.
In the code that validates log in details, I set the return type to TRUE so that anyone can log in, but I do have the code in place just incase you want to use the login details.
