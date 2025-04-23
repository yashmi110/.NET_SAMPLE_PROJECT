using System;
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Department { get; set; }

    public Employee(int id, string name, int age, string department)
    {
        Id = id;
        Name = name;
        Age = age;
        Department = department;
    }

    public virtual void DisplayDetails()
    {
        Console.WriteLine($"ID: {Id}, Name: {Name}, Age: {Age}, Department: {Department}");
    }
}