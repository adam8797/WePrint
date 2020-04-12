# Mappings

ViewModels and Create Models are done with AutoMapper Mappings.

In this directory, you'll find the following folder structure:

```
/ModelName/ModelName.cs
/ModelName/ModelNameViewModel.cs
/ModelName/ModelNameCreateModel.cs
/ModelName/ModelNameProfile.cs
```

## ModelName.cs

This is the Database Entity model. It *should* be annotated with as many attributes as possible, in order to correctly define the table. We don't like `nvarchar(MAX)` columns in our tables!

A model class *should* use the following annotations as liberally as possible:

```
[Key]
[MaxLength(int)]
[Required]
[NotMapped]
```

It *shall not* use `[ForeignKey]`, instead configure that in the `WePrintContext.cs`

It *shall* initalize all collections with the static initalizer

It *should* implement IIdentifiable<TKey>

If making a collection, the class *shall* use `IList<T>`

All Navigation properties *shall* be virtual to enable lazy loading. EF will throw up if it finds a non-virtual navigation property.

> Navigation properties are all the instances of `IList<T>` and any object references in the model


## ModelNameViewModel.cs

This is the view model, it is what will be presented to the API as response objects, and will generally be what the API client interacts with.

It *shall* contain the Id property that matches its DB Model

It *should* change all navigation properties to their keys. For example, a `User` property would change to a `Guid` property, and an `IList<User>` would become an `IList<Guid>`

## ModelNameCreateModel.cs

This is the creation model, used by POST and PUT requests. This may be the most limited of the models, as it should only contain information that a user could fill out in order to create a new entity using the API.

Items that are automatically assigned, *should* be omitted from this.

The class *shall not* contain an Id field.

## ModelNameProfile.cs

This is the AutoMapper profile that configures the mapping between all three above classes.

The following mappings *shall* be implemented:

1. ModelName => ModelNameViewModel
2. ModelNameViewModel => ModelName
3. ModelNameCreateModel => ModelName
4. TKey => ModelName
5. ModelName => TKey

Any other mappings *may* be implemented

Mapping 2. *shall* contain a call to `EqualityComparison()`

A template for the class is found here:

```c#
using System;
using AutoMapper;
using AutoMapper.EquivalencyExpression;

namespace WePrint.Models.ModelName
{
    public class ModelNameProfile : Profile
    {
        public ModelNameProfile()
        {
            CreateMap<ModelName, ModelNameViewModel>();
            CreateMap<ModelNameViewModel, ModelName>();
            CreateMap<ModelNameCreateModel, ModelName>();
            CreateMap<TKey, ModelName>().ConvertUsing<EntityConverter<TKey, ModelName>>();
            CreateMap<ModelName, TKey>().ConvertUsing(x => x.Id);
        }
    }
}
```
