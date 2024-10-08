﻿using Learnify_backend.Controllers;
using Learnify_backend.Entities;

namespace Learnify_backend.Services.CourseService
{
    public interface ICourseService
    {
        // Course
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> SearchCoursesAsync(CourseSearchParameters searchParameters);
        Task<Course> GetCourseByIdAsync(string id);
        Task<Course> CreateCourseAsync(CreateCourseRequest request);
        Task<string> UpdateCourseAsync(string id, UpdateCourseRequest request);
        Task DeleteCourseAsync(string id);

        // Module
        Task<IEnumerable<Module>> GetModulesByCourseAsync(string courseId);
        Task<Module> GetModuleByIdAsync(string id);
        Task<Module> CreateModuleAsync(CreateModuleRequest request);
        Task<string> UpdateModuleAsync(string id, UpdateModuleRequest request);
        Task DeleteModuleAsync(string id);

        // Lesson
        Task<IEnumerable<Lesson>> GetLessonsByModuleAsync(string moduleId);
        Task<Lesson> GetLessonByIdAsync(string id);
        Task<Lesson> CreateLessonAsync(CreateLessonRequest request);
        Task<string> UpdateLessonAsync(string id, UpdateLessonRequest request);
        Task DeleteLessonAsync(string id);

        // Quiz
        Task<Quiz> GetQuizByModuleAsync(string moduleId);
        Task<Quiz> GetQuizByIdAsync(string id);
        Task<Quiz> CreateQuizAsync(CreateQuizRequest request);
        Task DeleteQuizAsync(string id);

        // Question
        Task<IEnumerable<Question>> GetQuestionsByQuizAsync(string quizId);
        Task<Question> GetQuestionByIdAsync(string id);
        Task<Question> CreateQuestionAsync(CreateQuestionRequest request);
        Task<string> UpdateQuestionAsync(string id, UpdateQuestionRequest request);
        Task DeleteQuestionAsync(string id);
    }
}
