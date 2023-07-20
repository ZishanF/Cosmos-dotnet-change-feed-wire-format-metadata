using Newtonsoft.Json;

namespace ChangeFeedWireFormatResponse
{
    public class Family
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }
        [JsonProperty(PropertyName = "_rid")]
        public string Rid { get; set; }
        [JsonProperty(PropertyName = "_self")]
        public string Self { get; set; }
        [JsonProperty(PropertyName = "_attachments")]
        public string Attachments { get; set; }
        [JsonProperty(PropertyName = "_ts")]
        public string Ts { get; set; }

        public string LastName { get; set; }
        public Parent[] Parents { get; set; }
        public Child[] Children { get; set; }
        public Address Address { get; set; }
        public bool IsRegistered { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class FamilyWiredFormatResponse
    {
        [JsonProperty(PropertyName = "current")]
        public Family Current { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Metadata
    {
        [JsonProperty(PropertyName = "lsn")]
        public string Lsn { get; set; }
        [JsonProperty(PropertyName = "crts")]
        public string Crts { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Parent
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
    }

    public class Child
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public int Grade { get; set; }
        public Pet[] Pets { get; set; }
    }

    public class Pet
    {
        public string GivenName { get; set; }
    }

    public class Address
    {
        public string State { get; set; }
        public string County { get; set; }
        public string City { get; set; }
    }
}
