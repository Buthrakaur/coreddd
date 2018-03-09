﻿using System.Linq;
using System.Text;
using System.Web.Mvc;
using CoreDdd.Commands;
using CoreDdd.Queries;
using CoreUtils.Extensions;
using EmailMaker.Commands.Messages;
using EmailMaker.Controllers.BaseController;
using EmailMaker.Controllers.ViewModels;
using EmailMaker.Core;
using EmailMaker.Dtos;
using EmailMaker.Dtos.EmailTemplates;
using EmailMaker.Queries.Messages;
using MvcContrib;

namespace EmailMaker.Controllers
{
    public class TemplateController : AuthenticatedController
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;

        public TemplateController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor) : base(queryExecutor)
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        public ActionResult Index()
        {
            var emailTemplates = _queryExecutor.Execute<GetAllEmailTemplateQuery, EmailTemplateDetailsDto>(new GetAllEmailTemplateQuery{ UserId = UserId });           
            var model = new TemplateIndexModel { EmailTemplate = emailTemplates };
            return View(model);
        }

        public ActionResult Create()
        {            
            var createdEmailTemplateId = default(int);
            var command = new CreateEmailTemplateCommand { UserId = UserId };
            _commandExecutor.CommandExecuted += (sender, args) => createdEmailTemplateId = (int) args.Args;
            _commandExecutor.Execute(command);

            return this.RedirectToAction(a => a.Edit(createdEmailTemplateId));
        }

        public ActionResult Edit(int id)
        {
            var emailTemplate = _GetEmailTemplate(id);
            var model = new EmailTemplateEditModel {EmailTemplate = emailTemplate};
            return View(model);
        }

        private EmailTemplateDto _GetEmailTemplate(int id)
        {
            var templateMessage = new GetEmailTemplateQuery {EmailTemplateId = id};
            var templatePartMessage = new GetEmailTemplatePartsQuery { EmailTemplateId = id };

            var emailTemplateDtos = _queryExecutor.Execute<GetEmailTemplateQuery, EmailTemplateDto>(templateMessage);
            var emailTemplatePartDtos = _queryExecutor.Execute<GetEmailTemplatePartsQuery, EmailTemplatePartDto>(templatePartMessage);
            
            var emailTemplateDto = emailTemplateDtos.Single();
            emailTemplateDto.Parts = emailTemplatePartDtos;

            return emailTemplateDto;
        }

        [HttpPost]
        public void Save(SaveEmailTemplateCommand command)
        {
            _commandExecutor.Execute(command);
        }

        [HttpPost]
        public void CreateVariable(CreateVariableCommand command)
        {
            _commandExecutor.Execute(command);
        }

        [HttpPost]
        public void DeleteVariable(DeleteVariableCommand command)
        {
            _commandExecutor.Execute(command);
        }

        [HttpPost]
        public ActionResult GetEmailTemplate(int id)
        {
            return Json(_GetEmailTemplate(id));
        }

        public string GetHtml(int id)
        {
            var partMessage = new GetEmailTemplatePartsQuery { EmailTemplateId = id };
            var emailTemplatePartDtos = _queryExecutor.Execute<GetEmailTemplatePartsQuery, EmailTemplatePartDto>(partMessage);

            var sb = new StringBuilder();
            emailTemplatePartDtos.Each(part =>
            {
                switch (part.PartType)
                {
                    case PartType.Html:
                        sb.Append(part.Html);
                        break;
                    case PartType.Variable:
                        sb.Append(part.VariableValue);
                        break;
                    default:
                        throw new EmailMakerException("Unknown part type:" + part.PartType);
                }
            });
            return sb.ToString();
        }
    
    }
}