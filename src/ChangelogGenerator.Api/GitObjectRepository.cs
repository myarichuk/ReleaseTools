using System.Runtime.CompilerServices;
using LibGit2Sharp;

namespace ChangelogGenerator.Api;

public abstract class GitObjectRepository<TQueryResultItem, TQueryParameters>
{
    protected readonly Repository Repository;
    
    protected GitObjectRepository(Repository repository)
    {
        Repository = repository;
        AssertRepositoryInGoodState();
    }

    //template method pattern :)
    public abstract IQueryable<TQueryResultItem> Query();
    public abstract IQueryable<TQueryResultItem> Query(in TQueryParameters @params);

    private void AssertRepositoryInGoodState()
    {
        if (Repository.Info.CurrentOperation != CurrentOperation.None)
        {
            throw new RepositoryNotReadyException($"Repository is in the middle of a pending interactive operation. Cannot continue. (operation type = {Repository.Info.CurrentOperation})");
        }
    }

}