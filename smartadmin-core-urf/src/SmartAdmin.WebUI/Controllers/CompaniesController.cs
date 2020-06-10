﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Data.Models;
using SmartAdmin.Entity.Models;
using SmartAdmin.WebUI.Extensions;
namespace SmartAdmin.WebUI.Controllers
{
  public class CompaniesController : Controller
  {
    private readonly SmartDbContext _context;

    public CompaniesController(SmartDbContext context)
    {
      _context = context;
    }

    // GET: Companies
    public async Task<IActionResult> Index()
    {
      return View(await _context.Companies.ToListAsync());
    }
    public async Task<IActionResult> LoadData() {
      var draw = Request.Form["draw"].FirstOrDefault();
      var start =Convert.ToInt32( Request.Form["start"].FirstOrDefault());
      var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
      var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
      var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
      var searchValue = Request.Form["search[value]"].FirstOrDefault();


      //Paging Size (10,20,50,100)    
      int pageSize = length != null ? Convert.ToInt32(length) : 0;
      int skip = start != null ? Convert.ToInt32(start) : 0;
      int recordsTotal = 0;

      // Getting all compan data    
      var companyData = (from companies in _context.Companies
                          select companies);

      //Sorting    
      if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
      {
        companyData = companyData.OrderBy(sortColumn , sortColumnDir);
      }
      //Search    
      if (!string.IsNullOrEmpty(searchValue))
      {
        companyData = companyData.Where(m => m.Name == searchValue);
      }

      //total number of rows count     
      recordsTotal =await companyData.CountAsync();
      //Paging     
      var data = await companyData.Skip(skip).Take(pageSize).ToListAsync();
      //Returning Json Data    
      return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
  
      }
    // GET: Companies/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var company = await _context.Companies
          .FirstOrDefaultAsync(m => m.Id == id);
      if (company == null)
      {
        return NotFound();
      }

      return View(company);
    }

    // GET: Companies/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Companies/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Code,Address,Contect,PhoneNumber,RegisterDate,Id")] Company company)
    {
      if (ModelState.IsValid)
      {
        _context.Add(company);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(company);
    }

    // GET: Companies/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var company = await _context.Companies.FindAsync(id);
      if (company == null)
      {
        return NotFound();
      }
      return View(company);
    }

    // POST: Companies/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,Code,Address,Contect,PhoneNumber,RegisterDate,Id")] Company company)
    {
      if (id != company.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(company);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!CompanyExists(company.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      return View(company);
    }

    // GET: Companies/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var company = await _context.Companies
          .FirstOrDefaultAsync(m => m.Id == id);
      if (company == null)
      {
        return NotFound();
      }

      return View(company);
    }

    // POST: Companies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var company = await _context.Companies.FindAsync(id);
      _context.Companies.Remove(company);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool CompanyExists(int id)
    {
      return _context.Companies.Any(e => e.Id == id);
    }
  }
}