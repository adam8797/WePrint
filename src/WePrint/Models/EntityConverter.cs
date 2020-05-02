﻿using AutoMapper;
using WePrint.Data;

namespace WePrint.Models
{
    public class EntityConverter<TKey, TEntity> : ITypeConverter<TKey, TEntity> where TEntity : class, IIdentifiable<TKey> where TKey : struct
    {
        private readonly WePrintContext _context;

        public EntityConverter(WePrintContext context)
        {
            _context = context;
        }

        public TEntity Convert(TKey source, TEntity destination, ResolutionContext context)
        {
            return _context.Find<TEntity>(source);
        }
    }

    public class NullKeyEntityConverter<TKey, TEntity> : ITypeConverter<TKey?, TEntity> where TEntity : class, IIdentifiable<TKey> where TKey : struct
    {
        private readonly WePrintContext _context;

        public NullKeyEntityConverter(WePrintContext context)
        {
            _context = context;
        }

        public TEntity Convert(TKey? source, TEntity destination, ResolutionContext context)
        {
            if (source.HasValue)
                return _context.Find<TEntity>(source);
            return null;
        }
    }
}