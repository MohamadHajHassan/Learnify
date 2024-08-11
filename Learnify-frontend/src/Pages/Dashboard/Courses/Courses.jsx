import { Dialog, DialogPanel, DialogTitle, Transition, TransitionChild, Input, Field, Label } from '@headlessui/react'
import { useEffect, useState } from 'react'
import { toast } from 'react-toastify'
import CourseCard from './CourseCard'
import request from "./../../../utils/request"
import HelmetHandler from "./../../../utils/HelmetHandler"

const Courses = () => {
  const [courses, setCourses] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [duration, setDuration] = useState("");
  const [syllabus, setSyllabus] = useState("");
  const [instructorId, setInstructorId] = useState("");
  const [difficultyLevel, setDifficultyLevel] = useState("");
  const [categories, setCategories] = useState([]);
  const [categoryInput, setCategoryInput] = useState("");
  const [isOpen, setIsOpen] = useState(false);

  const fetchCourses = async () => {
    try {
      const { data } = await request.get("/Courses");
      setCourses(data);
    } catch (error) {
      console.error("Error fetching Courses:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const addCategory = () => {
    if (categoryInput.trim() !== "") {
      setCategories([...categories, categoryInput.trim()]);
      setCategoryInput("");
    }
  };

  const removeCategory = (index) => {
    setCategories(categories.filter((_, i) => i !== index));
  };

  const addNewCourseHandler = async () => {
    try {
      const formData = new FormData();
      formData.append("Title", title);
      formData.append("Description", description);
      formData.append("Categories", categories);
      formData.append("DifficultyLevel", difficultyLevel);
      formData.append("Duration", duration);
      formData.append("Syllabus", syllabus);
      formData.append("InstructorId", instructorId);
      // formData.append("PreRequisites", []);
      if (title.trim() === ""
        || description.trim() === ""
        || duration.trim() === ""
        || syllabus.trim() === ""
        || instructorId.trim() === ""
        || difficultyLevel === "") {
        return toast.error("Please fill all fields")
      }
      const config = {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      };
      await request.post("/Courses", formData, config)
      toast.success("Created Successfully!")
      fetchCourses()
    } catch (error) {
      return toast.error(error)
    } finally {
      setIsOpen(false)
    }
  }

  useEffect(() => {
    fetchCourses()
  }, [])


  return (
    <>
      <HelmetHandler title="Courses" />
      <div className='p-5 bg-gray-900 px-10 min-h-screen text-white'>
        <nav className='flex justify-between items-center'>
          <h1 className='text-2xl'>Courses</h1>
          <button
            onClick={() => { setIsOpen(true) }}
            className='inline-block rounded bg-indigo-600 px-8 py-3 text-sm font-medium text-white transition hover:scale-105'>
            Add New Course
          </button>
        </nav>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-5 mt-8">
          {
            courses.map((course) => {
              return (
                <CourseCard
                  course={course}
                  key={course.id}
                  fetchCourses={fetchCourses} />
              )
            })
          }
        </div>

        {isLoading ?
          (<p>Loading...</p>) :
          courses.length === 0 ?
            <p>No Courses Available!</p>
            : ""
        }

        <Transition appear show={isOpen}>
          <Dialog as="div" className="relative z-10 focus:outline-none" onClose={() => { setIsOpen(false) }}>
            <div className="fixed inset-0 z-10 w-screen overflow-y-auto">
              <div className="flex min-h-full items-center justify-center p-4">
                <TransitionChild
                  enter="ease-out duration-300"
                  enterFrom="opacity-0 transform-[scale(95%)]"
                  enterTo="opacity-100 transform-[scale(100%)]"
                  leave="ease-in duration-200"
                  leaveFrom="opacity-100 transform-[scale(100%)]"
                  leaveTo="opacity-0 transform-[scale(95%)]"
                >
                  <DialogPanel className="w-full max-w-lg rounded-xl shadow-2xl border border-white bg-gray-900 p-6">
                    <DialogTitle as="h1" className="font-bold text-xl mb-5 text-white">
                      Add new course
                    </DialogTitle>
                    <Field>
                      <Label className="text-sm/6 mr-1 font-medium text-white">Title</Label>
                      <Input value={title} onChange={(e) => { setTitle(e.target.value) }}
                        className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                      />
                    </Field>
                    <Field className="mt-3">
                      <Label className="text-sm/6 font-medium text-white">Description</Label>
                      <Input value={description} onChange={(e) => { setDescription(e.target.value) }}
                        className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                      />
                    </Field>

                    <Field className="mt-3">
                      <Label className="text-sm/6 font-medium text-white">Duration</Label>
                      <Input value={duration} onChange={(e) => { setDuration(e.target.value) }} type='number'
                        className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                      />
                    </Field>

                    <Field className="mt-3">
                      <Label className="text-sm/6 font-medium text-white">Syllabus</Label>
                      <Input value={syllabus} onChange={(e) => { setSyllabus(e.target.value) }}
                        className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                      />
                    </Field>

                    <Field className="mt-3">
                      <Label className="text-sm/6 font-medium text-white">Instructor Id</Label>
                      <Input value={instructorId} onChange={(e) => { setInstructorId(e.target.value) }}
                        className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                      />
                    </Field>

                    <Field className="mt-3">
                      <Label className="text-sm/6 font-medium text-white">Difficulty Level</Label>
                      <select
                        className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                        onChange={(e) => {
                          setDifficultyLevel(
                            e.target.value === "Beginner" ? 1
                              : e.target.value === "Intermediate" ? 2
                                : e.target.value === "Advanced" ? 3
                                  : "")
                        }}
                      >
                        <option selected value="Beginner" className='text-black'>Beginner</option>
                        <option className='text-black' value="Intermediate">Intermediate</option>
                        <option className='text-black' value="Advanced">Advanced</option>
                      </select>
                    </Field>

                    <Field className="mt-3">
                      <Label className="text-sm/6 font-medium text-white">Categories</Label>
                      <div className="flex items-center mt-2">
                        <Input value={categoryInput} onChange={(e) => { setCategoryInput(e.target.value) }}
                          className="block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                        />
                        <button onClick={addCategory} className="ml-2 inline-block rounded bg-indigo-600 px-3 py-1.5 text-sm font-medium text-white transition hover:scale-105">Add</button>
                      </div>
                      <ul className="mt-2">
                        {categories.map((category, index) => (
                          <li key={index} className="flex justify-between items-center bg-gray-800 p-2 rounded mt-1">
                            <span>{category}</span>
                            <button onClick={() => removeCategory(index)} className="text-red-500">Remove</button>
                          </li>
                        ))}
                      </ul>
                    </Field>

                    {/* Prereq */}

                    <div className="mt-7 grid grid-cols-1 md:grid-cols-2 gap-2 md:gap-5">
                      <button
                        onClick={addNewCourseHandler}
                        className='inline-block rounded bg-indigo-600 px-8 py-3 text-sm font-medium text-white transition hover:scale-105'>
                        Add
                      </button>
                      <button
                        onClick={() => { setIsOpen(false) }}
                        className='inline-block rounded bg-indigo-600 mr-5 px-8 py-3 text-sm font-medium text-white transition hover:scale-105'>
                        Cancel
                      </button>
                    </div>
                  </DialogPanel>
                </TransitionChild>
              </div>
            </div>
          </Dialog>
        </Transition>
      </div>
    </>
  )
}

export default Courses