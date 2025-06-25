<div align="center">

# Hotdesk Booking

</div>

## What is Hotdesk Booking

Hotdesk Booking is a web application designed to facilitate the booking of hot desks in an office environment. It allows users to view available desks, make reservations, and manage their bookings efficiently.

## Features of Hotdesk Booking
1. **Desk Booking**: Users can book desks for specific dates and times.

2. **Availability Check**: Users can check the availability of desks in real-time.

3. **User Management**: Users can create accounts, log in, and manage their profiles.


## Components of Hotdesk Booking

### Using Directive: All Directive used in the application

|  | Using Directive       | Function        |    
|--|---------------|-----------------------|
|1.| using System;  | Basic .NET types and base classes    | 
|2.| using System.Collections.Generic;  | Provides generic collection types | 
|3.| using System.LINQ;     | Enables LINQ capabilities for querying collection and databases    | 
|4.| using System.Threading.Tasks; | Provides type for asynchronous programming       |
|5.| using Microsoft.AspNetCore.Mvc; | Controllers and MVC features for ASP.NET          |
|6.| using Microsoft.AspNetCore.Http; | Provides types for handling HTTP context, requests, and responses         |
|7.| using Microsoft.EntityFrameworkCore; | Provides Entity Framework Core to work with database and migration |
|8.| using HotdeskAPI.data; | Import project namespoace for database contexrt |
|9.| using HotdeskAPI.Models; | Import model classes   |
|10.| using HotdeskAPI.component.Models; | Import other model classes, allowing to use the type directly |

## Important Attributes

### Attributes used in the application

| Attribute Name | Description |
|----------------|-------------|
| [Key] | Specifies the property that is the primary key of an entity. |
| [Required] | Indicates that a property must have a value. |
|[MaxLength(x)] |	Optimizes storage and prevents oversized data. |
| [ForeignKey] | Specifies a foreign key relationship between two entities. |
| virtual | Indicates that a property or method can be overridden in a derived class. |
| [JsonIgnore] |	Prevents circular reference issues in API responses. |

## Data Models

### Booking Model: Booking.cs

| Code line        | Function description           | 
|---------------|-----------------------|
| public class Booking | Defines the Booking class    | 
| [DatabaseGenerated(DatabaseGeneratedOption.Identity)] | Entity Framework auto-generate this field | 
| public int BookingId { get; set; }     | Booking ID - the unique ID for each booking.         | 
| public int DeskId { get; set; } |       Desk ID - the ID of the desk being booked.         |
| public int UserName { get; set; } |       Stores the name of the user who made the booking.         |
| public DateTime BookingDate { get; set; } | Stores the date when the booking takes place.         |
| public string DurationType { get; set; } = string.Empty; | Stores how long the booking is for (e.g."daily").         |
| public bool CheckedIn { get; set; } | Boolean flag to indicate if the user checked in.        |
| public DateTime? CheckInTime { get; set; } | Stores the time when the user checked in.         |
| public virtual Desk? Desk { get; set; } = null!; | Navigation property to the Desk entity.         |

### Desk Model: Desk.cs
| Code line        | Function description           |
|---------------|-----------------------|
| public class Desk | Defines the Desk class    |
| [DatabaseGenerated(DatabaseGeneratedOption.Identity)] | Entity Framework auto-generate this field |
| public int DeskId { get; set; } | Desk ID - the unique ID for each desk.         |