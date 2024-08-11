/* eslint-disable react/prop-types */
import { Fragment, useEffect, useState } from 'react'
import { Dialog, DialogPanel, DialogTitle, Transition, TransitionChild, Input, Field, Label } from '@headlessui/react'
import { Link } from 'react-router-dom'
import request from '../../../utils/request'
import { toast } from 'react-toastify'

const CourseCard = ({ course, fetchCourses }) => {
    const [isOpen, setIsOpen] = useState(false);
    const [title, setTitle] = useState(course.title);
    const [description, setDescription] = useState(course.description);
    const [duration, setDuration] = useState(course.duration);
    const [syllabus, setSyllabus] = useState(course.syllabus);
    const [instructorId, setInstructorId] = useState(course.instructorId);
    const [difficultyLevel, setDifficultyLevel] = useState(course.difficultyLevel)

    const [categories, setCategories] = useState(course.categories);
    const [categoryInput, setCategoryInput] = useState("");
    const [enrolledUsers, setEnrolledUsers] = useState(0);

    const updateCourseHandler = async () => {
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
            const config = {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            };
            await request.put(`/Courses/${course.id}?courseId=${course.id}`, formData, config)
            toast.success("Updated Successfully!")
            fetchCourses()
        } catch (error) {
            return toast.error(error)
        } finally {
            setIsOpen(false)
        }
    }

    const addCategory = () => {
        if (categoryInput.trim() !== "") {
            setCategories([...categories, categoryInput.trim()]);
            setCategoryInput("");
        }
    };
    const removeCategory = (index) => {
        setCategories(categories.filter((_, i) => i !== index));
    };

    const getNumberOfEnrolledUsers = async (courseId) => {
        try {
            const enrollments = await request.get(`/Enrollments/courseId/${courseId}`);
            setEnrolledUsers(enrollments.data.length || 0);
            return enrollments.data.length || 0;
        } catch (error) {
            console.log(error)
        }
    }

    useEffect(() => {
        if (course && course.id) {
            getNumberOfEnrolledUsers(course.id);
        }
    }, [course]);

    return (
        <Fragment>
            <div className="block relative rounded-lg p-4 shadow-sm bg-gray-800 shadow-gray-300">
                <div>
                    <div>
                        <dd className="font-medium text-3xl">{course?.title}</dd>
                    </div>
                </div>
                <div className="mt-4 flex items-center gap-8 text-xs">
                    <div className="inline-flex shrink-0 items-center gap-2">
                        <span className="text-xl">
                            <i className="fas fa-calendar-alt"></i>
                        </span>
                        <div className="mt-1.5 sm:mt-0">
                            <p className="text-gray-300">Created on</p>
                            <p className="font-medium">{course.createdOn}</p>
                        </div>
                    </div>
                </div>
                <div className="mt-4 flex items-center gap-8 text-xs">
                    <div className="inline-flex shrink-0 items-center gap-2">
                        <span className="text-xl">
                            <i className="fas fa-users"></i>
                        </span>
                        <div className="mt-1.5 sm:mt-0">
                            <p className="text-gray-300">Number of enrolled users</p>
                            <p className="font-medium">{enrolledUsers}</p>
                        </div>
                    </div>
                </div>
                <div className="mt-4 flex items-center gap-8 text-xs">
                    <div className="inline-flex shrink-0 items-center gap-2">
                        <span className="text-xl">
                            <i className="fas fa-tags"></i>
                        </span>
                        <div className="mt-1.5 sm:mt-0">
                            <p className="text-gray-300">Categories</p>
                            <p className="font-medium">{course.categories.join(', ')}</p>
                        </div>
                    </div>
                </div>
                <div className="mt-4 flex items-center gap-8 text-xs">
                    <div className="inline-flex shrink-0 items-center gap-2">
                        <span className="text-xl">
                            <i className="fas fa-signal"></i>
                        </span>
                        <div className="mt-1.5 sm:mt-0">
                            <p className="text-gray-300">Difficulty Level</p>
                            <p className="font-medium">
                                {course.difficultyLevel === 1
                                    ? "Beginner"
                                    : course.difficultyLevel === 2
                                        ? "Intermediate"
                                        : course.difficultyLevel === 3
                                            ? "Advanced"
                                            : ""}
                            </p>
                        </div>
                    </div>
                </div>
                <div className="mt-4 flex items-center gap-8 text-xs">
                    <div className="inline-flex shrink-0 items-center gap-2">
                        <span className="text-xl">
                            <i className="fas fa-clock"></i>
                        </span>
                        <div className="mt-1.5 sm:mt-0">
                            <p className="text-gray-300">Duration</p>
                            <p className="font-medium">{course.duration} hours</p>
                        </div>
                    </div>
                </div>
                <div className="grid grid-cols-1 shrink-0 justify-between items-center gap-2 mt-4 ">
                    <Link
                        to={`/dashboard/courses/${course?.id}`}
                        className='hover:bg-indigo-500 text-center hover:text-white transition-all border-indigo-600 border-2 text-gy-900 rounded-full text-base py-2 px-5 space-x-2 space-x-reverse'>
                        Details
                    </Link>
                    <Link
                        onClick={() => { setIsOpen(true) }}
                        className='bg-indigo-500 text-center text-slate-100 rounded-full text-base  py-2 px-5 space-x-2 space-x-reverse'>
                        Update
                    </Link>
                </div>
            </div>

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
                                        Update Course
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
                                            value={
                                                difficultyLevel === 1 ? "Beginner"
                                                    : difficultyLevel === 2 ? "Intermediate"
                                                        : difficultyLevel === 3 ? "Advanced"
                                                            : ""
                                            }
                                            className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                                            onChange={(e) => {
                                                setDifficultyLevel(
                                                    e.target.value === "Beginner" ? 1
                                                        : e.target.value === "Intermediate" ? 2
                                                            : e.target.value === "Advanced" ? 3
                                                                : "")
                                            }}
                                        >
                                            <option value="Beginner" className='text-black'>Beginner</option>
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
                                            onClick={updateCourseHandler}
                                            className='inline-block rounded bg-indigo-600 px-8 py-3 text-sm font-medium text-white transition hover:scale-105'>
                                            Update
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
        </Fragment>
    )
}

export default CourseCard