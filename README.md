# bank_ledger_interview

### Description
The program provided is an implementation of a simple personal bank ledger. It consist of a CLI and REST API which I created with the intention of experimentation (I have never created a REST API in C#). There is a minimalistic React frontend provided in FrontEnd/ which can be viewed in the browser locally. If the program is run with the argument "--webapi", it will execute the REST service on port 9000 : otherwise the CLI executes. I have the service setup to use my personal dev AWS RDS MySql DB for persistent storage.

### Implementation

#### Front-End_____________(V)

The frontend is developed in React and utilizes the npm module 'axios' to interface with the REST API. There is also a couple simple animations implemented with 'react-pose'.

#### Back-End______________(C)

The CLI backend is implemented purely in C# and leans heavily on the NuGet package MySql.Data. The design pattern loosely follows the mediator pattern. CommandDispatch.cs serves as a mediator decoupling the regex expression of a command, and its implementation. 

The REST version of the backend found in Web/ is able to reuse the various implementations of Command.cs by serving essentially the same role as CommandDispather.cs – decoupling request and implementation.

#### Persistent-Storage____(M)
The relational DB consist of three tables : Account, Balance, and History. Account contains an auto incremented column id which serves as foreign keys in Balance and History associating users and their columns. Additionally, there are stored procedures for withdraws, deposits, and account creation that log all interactions in the History table with the invoking users id. 







