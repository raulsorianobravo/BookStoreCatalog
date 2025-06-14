﻿using AutoMapper;
using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services;
using BookStoreCatalog_web.Services.IServices;
using BookStoreCatalog_web.ViewModel;
using BookStoreCatalogUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BookStoreCatalog_web.Controllers
{
    public class IssueController : Controller
    {

        private readonly IIssueService _issueService;
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;

        public IssueController(IIssueService issueService, IMapper mapper, IBookService bookService)
        {
            _issueService = issueService;
            _mapper = mapper;
            _bookService = bookService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IndexIssue()
        {
            List<IssueModelDTO> issueList = new List<IssueModelDTO>();

            var response = await _issueService.GetAllIssues<APIResponse>(HttpContext.Session.GetString(ClassDefinitions.SessionToken));
            if (response != null && response.IsSuccess)
            {
                issueList = JsonConvert.DeserializeObject<List<IssueModelDTO>>(Convert.ToString(response.Result));
            }

            return View(issueList);
        }

        public async Task<IActionResult> CreateIssue()
        {
            
            IssueViewModel issueViewModel = new IssueViewModel();
            var response = await _bookService.GetAllBooks<APIResponse>(HttpContext.Session.GetString(ClassDefinitions.SessionToken));

            if(response != null && response.IsSuccess)
            {
                issueViewModel.BookList = JsonConvert.DeserializeObject<List<BookModelDTO>>(Convert.ToString(response.Result))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Title,
                        Value = v.Id.ToString()
                    });
            }
            
            return View(issueViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIssue(IssueViewModel issueViewModel)
        {
            if (!ModelState.IsValid)
            {
                var response = await _issueService.CreateIssue<APIResponse>(issueViewModel.Issue, HttpContext.Session.GetString(ClassDefinitions.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["Success"] = "Issue Saved";
                    return RedirectToAction(nameof(IndexIssue));
                }
                else
                {
                    if(response.ErrorMessages.Count>0)
                    {
                        ModelState.AddModelError("ErrorMessage",response.ErrorMessages.FirstOrDefault());
                    }
                }
                
            }
            var res = await _bookService.GetAllBooks<APIResponse>(HttpContext.Session.GetString(ClassDefinitions.SessionToken));

            if (res != null && res.IsSuccess)
            {
                issueViewModel.BookList = JsonConvert.DeserializeObject<List<BookModelDTO>>(Convert.ToString(res.Result))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Title,
                        Value = v.Id.ToString()
                    });
            }

            return View(issueViewModel);
        }

        public async Task<IActionResult> UpdateIssue(int issueId)
        {
            IssueUpdateViewModel issueViewModel = new IssueUpdateViewModel();

            var response = await _issueService.GetIssue<APIResponse>(issueId, HttpContext.Session.GetString(ClassDefinitions.SessionToken));
            if (response != null && response.IsSuccess)
            {
                IssueModelDTO issue = JsonConvert.DeserializeObject<IssueModelDTO>(Convert.ToString(response.Result));
                issueViewModel.Issue = _mapper.Map<IsssueModelUpdateDTO>(issue);
            }

            response = await _bookService.GetAllBooks<APIResponse>(HttpContext.Session.GetString(ClassDefinitions.SessionToken));
            if (response != null && response.IsSuccess)
            {
                issueViewModel.BookList = JsonConvert.DeserializeObject<List<BookModelDTO>>(Convert.ToString(response.Result))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Title,
                        Value = v.Id.ToString()
                    });

                return View(issueViewModel);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIssue(IssueUpdateViewModel issueViewModel)
        {
            if (!ModelState.IsValid)
            {
                var response = await _issueService.UpdateIssue<APIResponse>(issueViewModel.Issue, HttpContext.Session.GetString(ClassDefinitions.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["Success"] = "Book Updated";
                    return RedirectToAction(nameof(IndexIssue));
                }
                else
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessage", response.ErrorMessages.FirstOrDefault());
                    }
                }

            }
            var res = await _bookService.GetAllBooks<APIResponse>(HttpContext.Session.GetString(ClassDefinitions.SessionToken));

            if (res != null && res.IsSuccess)
            {
                issueViewModel.BookList = JsonConvert.DeserializeObject<List<BookModelDTO>>(Convert.ToString(res.Result))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Title,
                        Value = v.Id.ToString()
                    });
            }

            return View(issueViewModel);
        }

        public async Task<IActionResult> DeleteIssue(int issueId)
        {
            IssueDeleteViewModel issueViewModel = new IssueDeleteViewModel();

            var response = await _issueService.GetIssue<APIResponse>(issueId, HttpContext.Session.GetString(ClassDefinitions.SessionToken));
            if (response != null && response.IsSuccess)
            {
                IssueModelDTO issue = JsonConvert.DeserializeObject<IssueModelDTO>(Convert.ToString(response.Result));
                issueViewModel.Issue = issue;
            }

            response = await _bookService.GetAllBooks<APIResponse>(HttpContext.Session.GetString(ClassDefinitions.SessionToken));
            if (response != null && response.IsSuccess)
            {
                issueViewModel.BookList = JsonConvert.DeserializeObject<List<BookModelDTO>>(Convert.ToString(response.Result))
                    .Select(v => new SelectListItem
                    {
                        Text = v.Title,
                        Value = v.Id.ToString()
                    });

                return View(issueViewModel);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIssue(IssueDeleteViewModel issueViewModel)
        {
            var response = await _issueService.DeleteIssue<APIResponse>(issueViewModel.Issue.issueId, HttpContext.Session.GetString(ClassDefinitions.SessionToken));
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Book Deleted";
                return RedirectToAction(nameof(IndexIssue));
            }
            return View(issueViewModel);
        }

    }
}
