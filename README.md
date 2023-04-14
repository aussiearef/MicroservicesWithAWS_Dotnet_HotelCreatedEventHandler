# Hotel Created Event Handler (Search Domain)


![Microservices with .NET and AWS ](https://img-b.udemycdn.com/course/750x422/2080118_8bbf_7.jpg "Microservices with .NET and AWS")



This Lambda Function is part of an online course called ["Build Microservices with .NET and Amazon Web Services"](https://www.udemy.com/course/build-microservices-with-aspnet-core-amazon-web-services/?referralCode=B288BF33506B34292176)

The Lambda function subscribes to an SNS topic and receives HotelCreatedEvent events from it. Then the events are published to an Elasticsarch instance to be used by a Search Docker microservice.
