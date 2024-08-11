import request from '../../utils/request';
import { useEffect, useState } from 'react';
import CourseCard from './CourseCard';

const Courses = () => {
  const [isLoading, setIsLoading] = useState(true);
  const [enrolledCourses, setEnrolledCourses] = useState([]);

  const getEnrolledCourses = async () => {
    const enrollments = JSON.parse(localStorage.getItem("enrollments")) || [];
    const courses = await Promise.all(
      enrollments.map(async enrollment => {
        const response = await request.get(`/Courses/${enrollment.courseId}`);
        return response.data;
      })
    );
    setIsLoading(false);
    setEnrolledCourses(courses);
  }
  useEffect(() => {
    getEnrolledCourses();
  }, []);

  return (
    <div className="container mx-auto bg-slate-100 px-6 text-gray-600 md:px-12 xl:px-6">
      <div className="mb-12 space-y-2 text-center">
        {/* <h2 className="text-3xl font-bold text-cyan-600 md:text-4xl mb-14">Enrolled Courses</h2> */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-9">
          {isLoading ? (
            <p>Loading...</p>
          ) : enrolledCourses.length === 0 ? (
            <p>No Courses Available!</p>
          ) : (
            enrolledCourses.map((course) => (
              <CourseCard
                course={course}
                key={course.id}
                getEnrolledCourses={getEnrolledCourses}
              />
            ))
          )}
        </div>
      </div>
    </div>
  )
}

export default Courses
