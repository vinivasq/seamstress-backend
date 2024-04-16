namespace Seamstress.Persistence.Contracts
{
  public interface IGeneralPersistence
  {
    void Add<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    void UpdateRange<T>(T[] entityArray) where T : class;
    void Delete<T>(T entity) where T : class;
    void DeleteRange<T>(T[] entity) where T : class;
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
    bool SaveChanges();
    Task<bool> SaveChangesAsync();
  }
}