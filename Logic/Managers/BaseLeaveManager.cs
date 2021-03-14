using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using Microsoft.AspNetCore.Identity;

namespace leave_management.Logic.Managers
{
    public class BaseLeaveManager
    {
        private protected readonly IMapper mapper;
        private protected readonly IUnitOfWork unitOfWork;

        public BaseLeaveManager(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
    }
}