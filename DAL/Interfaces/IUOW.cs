using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces.Contacts;
using DAL.Interfaces.Identity;
using DAL.Interfaces.Orders;
using DAL.Interfaces.Storage;
using Domain;
using Domain.Contacts;
using Domain.Orders;
using Domain.Storage;

namespace DAL.Interfaces
{
    public interface IUOW
    {
        //save pending changes to the data store
        void Commit();
        void RefreshAllEntities();

        //UOW Methods, that dont fit into specific repo

        //get repository for type
        T GetRepository<T>() where T : class;

        // standard autocreated repos, since we do not have any special methods in interfaces
        IEFRepository<ContactType> ContactTypes { get; }
        IEFRepository<PersonType> PersonTypes { get; }
        IEFRepository<OrderType> OrderTypes { get; }
        IEFRepository<StoredProduct> StoredProducts { get; }
        IEFRepository<ProductCategory> ProductCategories { get; }


            // Custom repos
        IContactRepository Contacts { get; }
        IPersonRepository Persons { get; }
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }

        // Identity repos, PK - int
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IUserRoleRepository UserRoles { get; }
        IUserClaimRepository UserClaims { get; }
        IUserLoginRepository UserLogins { get; }

    }
}