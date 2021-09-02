using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DnDWebAppMVC.Data
{
    public class TokenAuth
    {
        private readonly ApplicationDbContext _context;

        public TokenAuth(ApplicationDbContext context)
        {
            _context = context;
        }

        //public void LoginDealerUser(ClaimsPrincipal user)
        //{
        //    var guid = AuthHelper.GetOid(user);

        //    if (!String.IsNullOrWhiteSpace(guid))
        //    {
        //        Guid oid = Guid.Parse(guid);
        //        var checkUser = _context.DealerLeadUser.FirstOrDefault(x => x.AzureId == oid);

        //        if (checkUser == null)
        //            CreateDealerUser(user);
        //    }
        //}

        //public int? GetUserId(ClaimsPrincipal user)
        //{
        //    var guid = Guid.Parse(AuthHelper.GetOid(user));
        //    var checkUser = _context.DealerLeadUser.FirstOrDefault(x => x.AzureId == guid);

        //    return checkUser.Id;
        //}

        //public void CreateDealerUser(ClaimsPrincipal user)
        //{
        //    Guid oid = Guid.Parse(AuthHelper.GetOid(user));

        //    _context.DealerLeadUser.Add(new DealerLeadUser
        //    {
        //        AzureId = oid,
        //        CreateDate = DateTime.Now
        //    });
        //    _context.SaveChanges();
        //}
    }
}
