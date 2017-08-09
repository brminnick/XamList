using System;
namespace XamList.Shared
{
    public interface IBaseModel
    {
        string Id { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
