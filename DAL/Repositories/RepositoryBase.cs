﻿using DAL.Contexts;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected FitnessTrackerContext Context { get; set; }

    protected RepositoryBase(FitnessTrackerContext context)
    {
        Context = context;
    }

    public IQueryable<T> FindAll(bool trackChanges = false) =>
        trackChanges 
        ? Context.Set<T>() 
        : Context.Set<T>().AsNoTracking();

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false) =>
        trackChanges 
        ? Context.Set<T>().Where(expression) 
        : Context.Set<T>().Where(expression).AsNoTracking();

    public void Create(T entity) => Context.Set<T>().Add(entity);
    public void Update(T entity) => Context.Set<T>().Update(entity);
    public void Delete(T entity) => Context.Set<T>().Remove(entity);
}