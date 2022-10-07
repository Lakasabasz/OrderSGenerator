create table Role
(
    Roleid                    int auto_increment
        primary key,
    Rolename                  varchar(64) not null,
    GivingOrdersIndependently tinyint     not null,
    UserManagement            tinyint     not null,
    Admin                     tinyint     not null
);

create table Settings
(
    id         int         not null
        primary key,
    Post       varchar(10) not null,
    YearlyMode tinyint     not null
);

create table Stations
(
    StationId   int auto_increment
        primary key,
    StationName varchar(128) not null,
    constraint StationName_UNIQUE
        unique (StationName)
);

create table PhysicalLocations
(
    LocationId   int auto_increment
        primary key,
    LocationName varchar(128) not null,
    StationId    int          not null,
    constraint LocationName_UNIQUE
        unique (LocationName),
    constraint FK_PL_St
        foreign key (StationId) references Stations (StationId)
);

create index FK_PL_St_idx
    on PhysicalLocations (StationId);

create table Users
(
    Userid           int auto_increment
        primary key,
    Username         varchar(64)  not null,
    LastName         varchar(45)  not null,
    Roleid           int          not null,
    Password         varchar(128) not null,
    LoggedInTill     datetime     null,
    LoggedInFrom     varchar(15)  null,
    LoggedInLocation int          null,
    constraint FK_U_PL
        foreign key (LoggedInLocation) references PhysicalLocations (LocationId),
    constraint FK_U_R
        foreign key (Roleid) references Role (Roleid)
);

CREATE TABLE `OrdersS` (
                           `OrderSid` int(11) NOT NULL AUTO_INCREMENT,
                           `MinorNumber` int(11) NOT NULL,
                           `MajorNumber` int(11) NOT NULL,
                           `Authorized` int(11) NOT NULL,
                           `OnCommand` int(11) DEFAULT NULL,
                           `Status` varchar(45) NOT NULL,
                           `TrainDriverNumber` varchar(6) DEFAULT NULL,
                           `ForTrain` tinyint(4) NOT NULL,
                           `TrainNumber` varchar(6) NOT NULL,
                           `Date` date NOT NULL,
                           `SignalDriveOrder` tinyint(4) DEFAULT NULL,
                           `SemaphoreS1OutName` varchar(5) DEFAULT NULL,
                           `SemaphoreS1SingpostOutName` varchar(5) DEFAULT NULL,
                           `WithoutSemaphoreOutNumber` varchar(5) DEFAULT NULL,
                           `SemaphoreS1InName` varchar(5) DEFAULT NULL,
                           `SemaphoreS1SignpostInName` varchar(5) DEFAULT NULL,
                           `SemaphoreS1SpaceInName` varchar(5) DEFAULT NULL,
                           `WithoutSemaphoreInNumber` varchar(5) DEFAULT NULL,
                           `From` varchar(128) DEFAULT NULL,
                           `To` varchar(128) DEFAULT NULL,
                           `TrackNr` varchar(5) DEFAULT NULL,
                           `LastTrainNr` varchar(5) DEFAULT NULL,
                           `LastTrainDestination` varchar(128) DEFAULT NULL,
                           `LastTrainTime` varchar(6) DEFAULT NULL,
                           `Other` text DEFAULT NULL,
                           `StationId` int(11) NOT NULL,
                           `OrderHour` int(11) NOT NULL,
                           `OrderMinute` int(11) NOT NULL,
                           `TrainDriver` varchar(45) NOT NULL,
                           `TrainManager` varchar(45) DEFAULT NULL,
                           `Canceled` tinyint(4) NOT NULL,
                           PRIMARY KEY (`OrderSid`),
                           UNIQUE KEY `U_OS_MIN_MAJ_idx` (`MajorNumber`,`MinorNumber`),
                           KEY `FK_OS_U_A_idx` (`Authorized`),
                           KEY `FK_OS_U_OC_idx` (`OnCommand`),
                           KEY `FK_OS_S_S_idx` (`StationId`),
                           CONSTRAINT `FK_OS_S_S` FOREIGN KEY (`StationId`) REFERENCES `Stations` (`StationId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
                           CONSTRAINT `FK_OS_U_A` FOREIGN KEY (`Authorized`) REFERENCES `Users` (`Userid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
                           CONSTRAINT `FK_OS_U_OC` FOREIGN KEY (`OnCommand`) REFERENCES `Users` (`Userid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
                           CONSTRAINT `CHK_D1` CHECK ((`SemaphoreS1OutName` is not null) + (`SemaphoreS1SingpostOutName` is not null) + (`WithoutSemaphoreOutNumber` is not null) <= 1),
                           CONSTRAINT `CHK_D2` CHECK ((`SemaphoreS1InName` is not null) + (`SemaphoreS1SignpostInName` is not null) + (`SemaphoreS1SpaceInName` is not null) + (`WithoutSemaphoreInNumber` is not null) <= 1),
                           CONSTRAINT `CHK_ANY` CHECK (`SemaphoreS1OutName` is not null or `SemaphoreS1SingpostOutName` is not null or `WithoutSemaphoreOutNumber` is not null or `SemaphoreS1InName` is not null or `SemaphoreS1SignpostInName` is not null or `SemaphoreS1SpaceInName` is not null or `WithoutSemaphoreInNumber` is not null or `From` is not null or `To` is not null or `TrackNr` is not null or `LastTrainNr` is not null or `LastTrainDestination` is not null or `LastTrainTime` is not null),
                           CONSTRAINT `CHK_D3` CHECK ((`From` is not null) + (`To` is not null) + (`TrackNr` is not null) + (`LastTrainNr` is not null) + (`LastTrainDestination` is not null) + (`LastTrainTime` is not null) = 6 or (`From` is not null) + (`To` is not null) + (`TrackNr` is not null) + (`LastTrainNr` is not null) + (`LastTrainDestination` is not null) + (`LastTrainTime` is not null) = 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8