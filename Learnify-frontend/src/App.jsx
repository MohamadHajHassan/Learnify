import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom"
import Home from "./Pages/Home/Home"
import Layout from "./Pages/components/layout"
import Login from "./Pages/Login/Login"
import Register from "./Pages/Register/Register"
import Profile from "./Pages/Profile/Profile"
import Error from "./Pages/Error/Error"
import Courses from "./Pages/Profile/Courses"
import "video-react/dist/video-react.css"
import { useSelector } from "react-redux"
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import LayoutDash from "./Pages/Dashboard/components/LayoutDash"
import CoursesDash from "./Pages/Dashboard/Courses/Courses"
import UsersDash from "./Pages/Dashboard/Users/Users"
import Details from "./Pages/Profile/Details"

function App() {
  const { user } = useSelector(state => state.auth)
  return (
    <>
      <ToastContainer />
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<Home />} />
            <Route path="login" element={user ? <Navigate to="/" /> : <Login />} />
            <Route path="register" element={user ? <Navigate to="/" /> : <Register />} />
            <Route path="/me" >
              <Route element={!user ? <Navigate to="/" /> : <Profile />}>
                <Route path="profile" element={!user ? <Navigate to="/" /> : <Details />} />
                <Route path="courses" element={!user ? <Navigate to="/" /> : <Courses />} />
              </Route>
            </Route>
            <Route path="*" element={<Error />} />
          </Route>
          <Route path="/dashboard" element={user?.role === "Admin" ? <LayoutDash /> : <Navigate to="/" />} >
            <Route path="users" element={user?.role === "Admin" ? <UsersDash /> : <Navigate to="/" />} />
            <Route path="courses" element={user?.role === "Admin" ? <CoursesDash /> : <Navigate to="/" />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </>
  )
}

export default App
