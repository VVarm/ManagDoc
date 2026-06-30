public class Employee {
    public Guid Id { get; }
    public string Name { get; private set; }
    public string Phone { get; private set; }

    public Employee(string name, string phone) {
        Id = Guid.NewGuid();
        Name = name == null ? throw new ArgumentNullException(nameof(name)) : name;
        Phone = phone == null ? throw new ArgumentNullException(nameof(phone)) :
                                PhoneHelper.IsValidPhone(PhoneHelper.NormalizePhone(phone)) ? PhoneHelper.NormalizePhone(phone) :
                                    throw new ArgumentException("Invalid phone number", nameof(phone));
    }

    // для EF Core
    private Employee()
    {
        Name = string.Empty;
        Phone = string.Empty;
    }

    public void UpdateEmployee(string newName, string newPhone)
    {
        Name = newName == null ? throw new ArgumentNullException(nameof(newName)) : newName;
        Phone = newPhone == null ? throw new ArgumentNullException(nameof(newPhone)) :
                                PhoneHelper.IsValidPhone(PhoneHelper.NormalizePhone(newPhone)) ? PhoneHelper.NormalizePhone(newPhone) :
                                    throw new ArgumentException("Invalid phone number", nameof(newPhone));
    }
}