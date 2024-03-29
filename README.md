# LockToyApp
a toy app to show the POC of a smart door system 
Usage demo video https://youtu.be/dPjCnVGX5us


#Overview <br/>
In the solution folder there are four project folders: ToyContracts, SimulatedDoor, LockToyApp and DoorOperationFunc The ToyContract project contains the definitions across the whole application.

The SimulatedDoor project is a modified console iot hub client application from Microsoft example snippet, it is supposed to run inside the lock OS to receive the commands from the iot Hub. <br/>
<br/>
The LockToyApp is the web api application which receives the commands to open the door, check the operation history of a specific door, or get user information for the authorized user. There are currently three routes: <br/>
POST /api/lockoperation/authenticate <br/>
GET /api/lockoperation/user <br/>
POST /api/lockoperation/opendoor <br/>
GET /api/lockoperation/doorhistory
<br/>
<br/>
The DoorOperationFunc project is an Azure Function App which receives the commands message from the service bus and forward it to the iot hub, meanwhile it also consolidates the operations inside the Azure Cosmos Container

#Limitations <br/>
Due to the restriction of AAD of the company and time limit,  I am not able to setup multi-tenants and make use of AAD or LDAP to manage the authorization. Hence, I use a jwt authentication middleware to do the token authentication. Users have to make call to the authentication endpoint first to fetch a valid token then include it in the folow up requests in the bearer token header. 

For this toy app and demo purpose, I created two users’ credentials inside a Azure SQL Database which has been created during initilization of the database. When making api request, only correct user name and password should be proviced through http headers, otherwise either 401 or null results will be returned.  <br/> 
One normal user with the user name "User" (password: 1234567)has only access to two doors (iot devices with the id "87851188-1d8d-4330-875b-ddbdd876b867" and "55d0d901-2db9-4c47-b858-78334b8fc043") <br>

One elevated user with the user name "Admin" (password: 1234567admin) has access not only the normal user doors but also to 3 additional doors (iot devices with the id "4c903e6a-89e7-44ea-8490-3443c9dc9614" ,"3e4e19f2-5cee-4b23-b423-c98f5073ef89" and "64a6af7a-2c8c-42e9-8ba6-19a4dfe62f86") 

For CI/CD, the solution was planned to be built and deployed using Azure DevOp pipelines, due to time limit, this was not yet included, it will be implemented in the future. 

#Overall work flow <br/>

web api (validate user, query sql table, query cosmos db) &rarr; send message to service bus queue &rarr; azure function listens at the service bus queue &rarr; azure function consolidate the operation records in cosmos db and send processed message to iot hub &rarr; iot hub forward commands to the iot device

![image](https://user-images.githubusercontent.com/3876170/205245434-50ee5393-93f6-46c9-b27f-e334dc0334d1.png)
