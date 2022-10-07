using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Generator_rozkazów_S.Models;

public class OrderS
{
    public int OrderSid { get; set; }
    public int MinorNumber { get; set; }
    public int MajorNumber { get; set; }
    
    [Column("Authorized"), ForeignKey("Authorized")]
    public int? AuthorizedId { get; set; }
    public User? Authorized { get; set; }
    
    [Column("OnCommand"), ForeignKey("OnCommand")]
    public int? OnCommandId { get; set; }
    public User? OnCommand { get; set; }
    public string Status { get; set; } = null!;
    public string? TrainDriverNumber { get; set; }
    public bool ForTrain { get; set; }
    public string TrainNumber { get; set; } = null!;
    public DateOnly Date { get; set; }
    public bool? SignalDriveOrder { get; set; }
    public string? SemaphoreS1OutName { get; set; }
    public string? SemaphoreS1SingpostOutName { get; set; }
    public string? WithoutSemaphoreOutNumber { get; set; }
    public string? SemaphoreS1InName { get; set; }
    public string? SemaphoreS1SignpostInName { get; set; }
    public string? SemaphoreS1SpaceInName { get; set; }
    public string? WithoutSemaphoreInNumber { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string? TrackNr { get; set; }
    public string? LastTrainNr { get; set; }
    public string? LastTrainDestination { get; set; }
    public string? LastTrainTime { get; set; }
    public string? Other { get; set; }
    [Column("StationId"), ForeignKey("Station")]
    public int StationId { get; set; }
    public Station Station { get; set; } = null!;
    public int OrderHour { get; set; }
    public int OrderMinute { get; set; }
    public string TrainDriver { get; set; } = null!;
    public string? TrainManager { get; set; }
    public bool Canceled { get; set; }

    public bool CompareContent(OrderS orderRecord)
    {
        return MinorNumber == orderRecord.MinorNumber &&
               MajorNumber == orderRecord.MajorNumber &&
               TrainDriverNumber == orderRecord.TrainDriverNumber &&
               ForTrain == orderRecord.ForTrain &&
               TrainNumber == orderRecord.TrainNumber &&
               Date == orderRecord.Date &&
               SignalDriveOrder == orderRecord.SignalDriveOrder &&
               SemaphoreS1OutName == orderRecord.SemaphoreS1OutName &&
               SemaphoreS1SingpostOutName == orderRecord.SemaphoreS1SingpostOutName &&
               WithoutSemaphoreOutNumber == orderRecord.WithoutSemaphoreOutNumber &&
               SemaphoreS1InName == orderRecord.SemaphoreS1InName &&
               SemaphoreS1SignpostInName == orderRecord.SemaphoreS1SignpostInName &&
               SemaphoreS1SpaceInName == orderRecord.SemaphoreS1SpaceInName &&
               WithoutSemaphoreInNumber == orderRecord.WithoutSemaphoreInNumber &&
               From == orderRecord.From &&
               To == orderRecord.To &&
               TrackNr == orderRecord.TrackNr &&
               LastTrainNr == orderRecord.LastTrainNr &&
               LastTrainDestination == orderRecord.LastTrainDestination &&
               LastTrainTime == orderRecord.LastTrainTime &&
               Other == orderRecord.Other &&
               Station.Name == orderRecord.Station.Name &&
               OrderHour == orderRecord.OrderHour &&
               OrderMinute == orderRecord.OrderMinute &&
               TrainDriver == orderRecord.TrainDriver &&
               TrainManager == orderRecord.TrainManager &&
               Canceled == orderRecord.Canceled;
    }

    public VMOrderS ToVMOrderS()
    {
        return new VMOrderS()
        {
            Date = Date,
            OrderNumber = MinorNumber,
            IsedrSet = Authorized,
            FromOrderSet = OnCommand,
            TrainDriverNumber = TrainDriverNumber,
            ForTrain = ForTrain,
            TrainNumber = TrainNumber,
            SignalDriveOrder = SignalDriveOrder,
            SemaphoreS1OutName = SemaphoreS1OutName == string.Empty ? null : SemaphoreS1OutName,
            SemaphoreS1SignpostOutName = SemaphoreS1SingpostOutName == string.Empty ? null : SemaphoreS1SingpostOutName,
            WithoutSemaphoreOutNumber = WithoutSemaphoreOutNumber == string.Empty ? null : WithoutSemaphoreOutNumber,
            SemaphoreS1InName = SemaphoreS1InName == string.Empty ? null : SemaphoreS1InName,
            SemaphoreS1SignpostInName = SemaphoreS1SignpostInName == string.Empty ? null : SemaphoreS1SignpostInName,
            SemaphoreS1SpaceInName = SemaphoreS1SpaceInName == string.Empty ? null : SemaphoreS1SpaceInName,
            WithoutSemaphoreInNumber = WithoutSemaphoreInNumber == string.Empty ? null : WithoutSemaphoreInNumber,
            From = From == string.Empty ? null : From,
            To = To == string.Empty ? null : To,
            TrackNr = TrackNr == string.Empty ? null : TrackNr,
            LastTrainNr = LastTrainNr == string.Empty ? null : LastTrainNr,
            LastTrainDestination = LastTrainDestination == string.Empty ? null : LastTrainDestination,
            LastTrainTime = LastTrainTime == string.Empty ? null : LastTrainTime,
            Other = Other == string.Empty ? null : Other,
            Station = Station,
            Hour = OrderHour,
            Minute = OrderMinute,
            TrainDriver = TrainDriver,
            TrainManager = TrainManager == string.Empty ? null : TrainManager,
            Canceled = Canceled
        };
    }
}