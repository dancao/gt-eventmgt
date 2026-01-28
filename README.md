# REST API for a simplified ticketing system
- Technologies: Net Core API, .Net 10, EF Core, MS Test, Hangfire, MemoryCache, JWT Authentication, SQL Lite
- Design Patterns: DI, Circuit Breaker pattern, Asynchronous Request-Reply pattern, Singleton
- Global Exception handling
- you can get my postman colltion for integration test at "Others" section
## Event Management API
- Access "/swagger/index.html" to view all endpoints
### Auth
- POST /api/Auth/login
```
Request body
{
  "username": "xxx",
  "password": "xxx"
}
Reponse
{
    "token": "xxxxx",
    "expires": "2026-01-28T17:03:01Z"
}
```
### Event
- Create event: POST /api/Event
```
Request body
{
    "name": "Event1",
    "description": "DESC",
    "eventDate": "2026-06-01T12:05:09Z",
    "duration": 5,
    "venueId": 3,    
    "ticketTypes": [
        {
            "name": "VIP",
            "totalAvailable": 5,
            "remaining": 5,
            "pricingTierId": 1
        },
        {
            "name": "General Admission",
            "totalAvailable": 20,
            "remaining": 20,
            "pricingTierId": 2
        }
    ]
}
Reponse
{
    "status": 0, // 0 is success, 1: is failed
    "data": {
        // event object
    },
    "errorMessage": ""
}

```
- Update event: PUT /api/Event
> same request with create but you need to provide ID
- Delete event: DELETE /api/Event/{id}
- Get Event By ID: GET /api/Event/{id}?incVenue=Y&incTicketTypes=Y
> incVenue=Y => return Venue info details
> incTicketTypes=Y => return TicketTypes and all sold Tickets along with an Event
### Ticket Management
- Purchase Tickets: POST /api/Event/ticket
```
Request Body
{
    "ticketTypeId": 1,
    "buyerName": "Jon Snow",
    "quantity": 8,
    "totalCost": 45.6
}
Response
{
    "status": 0, // 1 if failed
    "data": true, // false if failed
    "message": "Purchase successful." // if failed: "Purchase failed, please try again later."
}
```
- GetTicketsAvailability: GET /api/Event/tickets?pageNumber=1&pageSize=15
> return all events with ticket types info, apply apgination
### Reporting
> Using async Request-Reply Pattern + Hangfire to handle report generating in the background
1. Start requesting: POST /api/Reports?reportName=TicketSalesByEventReport
> if success, you can get the "status link" in response header:Location to use for step #2
2. Get Status:  GET /api/Reports/status/{jobId}
> {jobId} was returned back from Step #1, if the job is completed, you will receive as below and you can execute step #3 to get Report data
```
{
    "status": "Completed",
    "resultUrl": "results/36bc2cdd-805a-4499-b4de-a63d3743439b"
}
```
3. Get Report: GET /api/Reports/results/{jobId}
```
{
    "status": 0,
    "data": [
        {
            "id": 7,
            "eventId": 1,
            "eventName": "Event 1",
            "ticketsCount": 4,
            "ticketsTotalCost": 119.2,
            "createdDate": "2026-01-28T16:09:14.027",
            "createdBy": "TicketSalesByEventReportAsync",
            "reportRequestId": "36bc2cdd-805a-4499-b4de-a63d3743439b"
        },
        {
            "id": 8,
            "eventId": 2,
            "eventName": "Event 2",
            "ticketsCount": 0,
            "ticketsTotalCost": 0.0,
            "createdDate": "2026-01-28T16:09:14.027",
            "createdBy": "TicketSalesByEventReportAsync",
            "reportRequestId": "36bc2cdd-805a-4499-b4de-a63d3743439b"
        },
        {
            "id": 9,
            "eventId": 3,
            "eventName": "Event 3",
            "ticketsCount": 1,
            "ticketsTotalCost": 45.6,
            "createdDate": "2026-01-28T16:09:14.027",
            "createdBy": "TicketSalesByEventReportAsync",
            "reportRequestId": "36bc2cdd-805a-4499-b4de-a63d3743439b"
        }
    ],
    "message": ""
}
```
### Future Enhancements
- Add Refund, Cancel tickets
- Add Front End app (Angular or React)
- Add Docker
- Deploy to Azure/AWS
### Others
- You can use Venue API, Pricing API to manage Venue and PricingTier.
- You need to use Authentication (Login) to use Venue API and Pricing API
- [Postman collection](https://github.com/dancao/gt-eventmgt/blob/64eb3d7a7527ebaf0382aad7ec2880d43d38dc70/EventMgt%20Testing.postman_collection.json)

**Happy Programing** :+1:
