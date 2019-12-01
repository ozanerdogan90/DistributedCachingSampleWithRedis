using System.Collections.Generic;

namespace DistributedCachingSampleWithRedis.Repositories
{
    public interface IDummyDataRepository
    {
        List<string> GetValues();
    }

    public class DummyDataRepository: IDummyDataRepository
    {
        public List<string> GetValues()
        {
            return new List<string>() { "IamNewMember", "NewInTown" };
        }
    }
}
