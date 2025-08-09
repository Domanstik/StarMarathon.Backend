namespace StarMarathon.Domain.Entities;

public sealed class Employee : Entity<Guid> {
    public long     TgId         { get; private set; }
    public string?  TgUsername   { get; private set; }
    public string   Phone        { get; private set; } = default!;
    public string   Language     { get; private set; } = "ru";
    public string?  AvatarUrl    { get; private set; }
    public ICollection<EmployeeGroup> Groups { get; private set; } = new List<EmployeeGroup>();
    public ICollection<EmployeeTaskStatus> TaskStatuses { get; private set; } = new List<EmployeeTaskStatus>();
    private Employee() { }
    public Employee(long tgId, string phone, string language) {
        Id = Guid.NewGuid(); TgId = tgId; Phone = phone; Language = language;
    }
}

public sealed class Group : Entity<Guid> {
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public ICollection<EmployeeGroup> EmployeeGroups { get; set; } = new List<EmployeeGroup>();
    public ICollection<TaskGroup>     TaskGroups     { get; set; } = new List<TaskGroup>();
    private Group() { }
    public Group(string code,string name){Id=Guid.NewGuid();Code=code;Name=name;}
}

public sealed class Task : Entity<Guid> {
    public string  Name                { get; private set; } = default!;
    public string  Description         { get; private set; } = default!;
    public string  Language            { get; private set; } = "ru";
    public int     WinningReward       { get; private set; }
    public int     ParticipationReward { get; private set; }
    public DateTime Created { get; private set; }
    public DateTime End     { get; private set; }
    public ICollection<TaskGroup>          TaskGroups       { get; } = new List<TaskGroup>();
    public ICollection<EmployeeTaskStatus> EmployeeStatuses { get; } = new List<EmployeeTaskStatus>();
    private Task() { }
    public Task(string name,string descr,string lang,int win,int part,DateTime end){
        Id=Guid.NewGuid();Name=name;Description=descr;Language=lang;
        WinningReward=win;ParticipationReward=part;Created=DateTime.UtcNow;End=end;
    }
}

public sealed class Product   : Entity<Guid> { public string Name {get;private set;}=default!; public string Description{get;private set;}=default!; public int Price{get;private set;} public string ImageUrl{get;private set;}=default!; private Product(){} public Product(string n,string d,int p,string img){Id=Guid.NewGuid();Name=n;Description=d;Price=p;ImageUrl=img;} }
public sealed class Notification:Entity<Guid>{public Guid EmployeeId{get;private set;} public string Description{get;private set;}=default!;public bool IsRead{get;private set;} public DateTime Created{get;private set;} private Notification(){} public Notification(Guid e,string d){Id=Guid.NewGuid();EmployeeId=e;Description=d;Created=DateTime.UtcNow;} public void MarkRead()=>IsRead=true;}
public sealed class EmployeeGroup{public Guid EmployeeId{get;set;} public Employee Employee{get;set;}=default!;public Guid GroupId{get;set;} public Group Group{get;set;}=default!;}
public sealed class TaskGroup   {public Guid TaskId{get;set;}    public Task Task {get;set;}=default!; public Guid GroupId{get;set;} public Group Group{get;set;}=default!;}
public sealed class EmployeeTaskStatus{public Guid EmployeeId{get;set;} public Employee Employee{get;set;}=default!; public Guid TaskId{get;set;} public Task Task {get;set;}=default!; public bool IsCompleted{get;set;} public DateTime? CompletedAt{get;set;}}
