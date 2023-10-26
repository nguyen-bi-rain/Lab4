using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThucHanh4.Data;
using ThucHanh4.Models;
using X.PagedList;

namespace ThucHanh4.Controllers
{
    public class LearnerController : Controller
    {
        public readonly SchoolContext _context;

        public LearnerController(SchoolContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? mid,int? page)
        {
            if (mid == null)
            {
                int pageSize = 2;
                int pageNum =  page == null || page < 0 ? 1 : page.Value;
                var learners = _context.Learners.AsNoTracking().Include(m => m.Major).ToList();
                PagedList<Learner> list  =  new PagedList<Learner>(learners,pageNum,pageSize);
                return View(list);
            }
            
            else
            {
                int pageSize = 2;
                int pageNum = page == null || page < 0 ? 1 : page.Value;
               
                var learners = _context.Learners.Where(l => l.MajorID == mid).Include(m => m.Major).ToList();
                PagedList<Learner> list = new PagedList<Learner>(learners, pageNum, pageSize);
                return View(learners);
            }
            
            
        }
        public IActionResult Create()
        {
            var majors = new List<SelectListItem>(); //cách 1
            foreach (var item in _context.Majors)
            {
                majors.Add(new SelectListItem
                {
                    Text = item.MajorName,

                    Value = item.MajorID.ToString()
                });

            }
            ViewBag.MajorID = majors;
            ViewBag.MajorID = new SelectList(_context.Majors, "MajorID", "MajorName"); //cách 2
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
        {
            ViewBag.MajorID = new SelectList(_context.Majors, "MajorID", "MajorName");
            if (ModelState.IsValid)
            {
                _context.Learners.Add(learner);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            //lại dùng 1 trong 2 cách tạo SelectList gửi về View để hiển thị danh sách Majors
            
            return View();
        }
        public IActionResult Delete(int? id)
        {
            var majors = new List<SelectListItem>(); //cách 1
            foreach (var item in _context.Majors)
            {
                majors.Add(new SelectListItem
                {
                    Text = item.MajorName,

                    Value = item.MajorID.ToString()
                });

            }
            ViewBag.MajorID = majors;
            ViewBag.MajorID = new SelectList(_context.Majors, "MajorID", "MajorName");
            var learners = _context.Learners.Include(l => l.Major).Include(e => e.Enrollments).FirstOrDefault(x => x.LearnerID == id);

            if (learners == null)
            {
                return NotFound();
            }
            if(learners.Enrollments.Count > 0)
            {
                return Content("Cant delete this learner");
            }
            return View(learners);
        }
        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            var learners = await _context.Learners.FindAsync(id);
            if(learners != null)
            {
                _context.Learners.Remove(learners);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var majors = new List<SelectListItem>(); //cách 1
            foreach (var item in _context.Majors)
            {
                majors.Add(new SelectListItem
                {
                    Text = item.MajorName,

                    Value = item.MajorID.ToString()
                });

            }
            ViewBag.MajorID = majors;
            ViewBag.MajorID = new SelectList(_context.Majors, "MajorID", "MajorName");
            var learners = await  _context.Learners.FindAsync(id);
            if(id == null)
            {
                return NotFound();
            }
            return View(learners);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
        {   
            learner.LearnerID = id;
            if (id != learner.LearnerID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Learners.Update(learner);
                    await _context.SaveChangesAsync();
                }catch(DbUpdateConcurrencyException ex)
                {
                    if(!LearnerExist(learner.LearnerID))
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
            ViewBag.MajorID = new SelectList(_context.Majors, "MajorID", "MajorName", learner.MajorID);
            return View(learner);
        }
        private bool LearnerExist(int id)
        {
            return (_context.Learners?.Any(e => e.LearnerID == id)).GetValueOrDefault();
        }

        public IActionResult LearnerByMajorID(int mid)
        {
            var learners = _context.Learners.Where(l => l.MajorID == mid).Include(m => m.Major).ToList();
            return PartialView("LearnerTable", learners);
        }
    }
}
