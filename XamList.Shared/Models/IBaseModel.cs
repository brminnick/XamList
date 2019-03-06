using System;
namespace XamList.Shared
{
    public interface IBaseModel
    {
        string Id { get; }
        DateTimeOffset UpdatedAt { get; }
    }
}
