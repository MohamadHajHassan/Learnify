/* eslint-disable react/prop-types */
import { Fragment } from 'react';
import { useSelector } from 'react-redux';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import request from '../../utils/request';

const CourseCard = ({ course, fetchCourses }) => {
    const { user } = useSelector(state => state.auth);

    const enrollHandler = async (courseId) => {
        try {
            const formData = new FormData();
            formData.append("UserId", user.id);
            formData.append("CourseId", courseId);
            const config = {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            };
            await request.post(`/Enrollments`, formData, config);
            toast.success("Enrolled Successfully!");
            const enrollments = await request.get(`/Enrollments/userId/${user.id}`);
            localStorage.setItem("enrollments", JSON.stringify(enrollments.data));
            fetchCourses();
        } catch (error) {
            toast.error(error.message);
        }
    };

    const isEnrolled = (courseId) => {
        const enrollments = JSON.parse(localStorage.getItem("enrollments")) || [];
        return enrollments.some(enrollment => enrollment.courseId === courseId);
    };

    return (
        <Fragment>
            <div className="block relative rounded-lg p-6 shadow-sm bg-white shadow-gray-300">
                <div className="text-left">
                    <h3 className="font-medium text-3xl">{course?.title}</h3>
                </div>
                <div className="mt-4 flex items-center gap-8 text-xs justify-start">
                    <div className="inline-flex shrink-0 items-center gap-2">
                        <span className="text-xl">
                            <i className="fas fa-calendar-alt"></i>
                        </span>
                        <div className="mt-1.5 sm:mt-0 text-left">
                            <p className="text-gray-400">Created on</p>
                            <p className="font-medium">{course.createdOn}</p>
                        </div>
                    </div>
                </div>
                <div className="mt-4 text-left">
                    <p className="text-gray-400">Categories</p>
                    <p className="font-medium">{course.categories.join(', ')}</p>
                </div>
                <div className="mt-4 text-left">
                    <p className="text-gray-400">Difficulty Level</p>
                    <p className="font-medium">
                        {course.difficultyLevel === 1 ? "Beginner" :
                         course.difficultyLevel === 2 ? "Intermediate" :
                         course.difficultyLevel === 3 ? "Advanced" : ""}
                    </p>
                </div>
                <div className="mt-4 text-left">
                    <p className="text-gray-400">Duration</p>
                    <p className="font-medium">{course.duration} hours</p>
                </div>
                <div className="grid grid-cols-1 shrink-0 justify-start items-center gap-2 mt-4">
                    <Link
                        to={`/dashboard/courses/${course?.id}`}
                        className='hover:bg-indigo-500 text-center hover:text-white transition-all border-indigo-600 border-2 text-gray-900 rounded-full text-base py-2 px-5 space-x-2 space-x-reverse'>
                        Details
                    </Link>
                    {!user ? (
                        <Link
                            to="/login"
                            className='bg-indigo-500 text-center text-slate-100 rounded-full text-base py-2 px-5 space-x-2 space-x-reverse'>
                            Login to Enroll
                        </Link>
                    ) : user.role === "Student" ? (
                        isEnrolled(course.id) ? (
                            <p className='text-green-500 font-semibold'>Enrolled</p>
                        ) : (
                            <button
                                onClick={() => enrollHandler(course.id)}
                                className='bg-indigo-500 text-center text-slate-100 rounded-full text-base py-2 px-5 space-x-2 space-x-reverse'>
                                Enroll
                            </button>
                        )
                    ) : null}
                </div>
            </div>
        </Fragment>
    );
};

export default CourseCard;