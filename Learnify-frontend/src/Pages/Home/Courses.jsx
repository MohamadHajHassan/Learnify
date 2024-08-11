import { useState, useEffect } from 'react';
import request from '../../utils/request';
import CourseCard from './CourseCard';

const Courses = () => {
    const [courses, setCourses] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

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

    useEffect(() => {
        fetchCourses();
    }, []);

    return (
        <div className="container mx-auto py-16 bg-slate-100 px-6 text-gray-600 md:px-12 xl:px-6">
            <div className="mb-12 space-y-2 text-center">
                <h2 className="text-3xl font-bold text-cyan-600 md:text-4xl mb-14">Courses</h2>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-9 mt-8">
                    {isLoading ? (
                        <p>Loading...</p>
                    ) : courses.length === 0 ? (
                        <p>No Courses Available!</p>
                    ) : (
                        courses.map((course) => (
                            <CourseCard
                                course={course}
                                key={course.id}
                                fetchCourses={fetchCourses}
                            />
                        ))
                    )}
                </div>
            </div>
        </div>
    );
};

export default Courses;