﻿using E_Commerse.Core.Entities;
using System.Linq.Expressions;

namespace E_Commerce.Service.Abstract
{
    public interface IService<T> where T : class, IEntity, new()
    {
        // SENKRON
        List<T> GetAll();
        List<T> GetAll(Expression<Func<T, bool>> expression);
        IQueryable<T> GetQueryable();
        T Get(Expression<Func<T, bool>> expression);
        T Find(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        int saveChanges();

        // ASENKRON
        Task<T> FindAsync(int id);
        Task<T> GetAsync(Expression<Func<T, bool>> expression);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task<int> saveChangesAsync();
    }
}
