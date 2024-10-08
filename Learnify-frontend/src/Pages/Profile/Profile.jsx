import { NavLink, Outlet } from "react-router-dom"
import "./profile.css"
import HelmetHandler from "./../../utils/HelmetHandler"

const Profile = () => {
  return (
    <>
      <HelmetHandler title="Profile" />

      <div className='bg-slate-100 p-10'>
        <div className="flex custom-scroll mb-7 overflow-x-auto overflow-y-hidden border-b border-gray-200 whitespace-nowrap dark:border-gray-700">
          <NavLink to="/me/profile" className="inline-flex TabLink items-center h-10 px-4 -mb-px text-sm text-center bg-transparent border-b-2 sm:text-base dark:border-blue-400 dark:text-blue-300 whitespace-nowrap focus:outline-none">
            Profile
          </NavLink>
          <NavLink to="/me/courses" className="inline-flex TabLink items-center h-10 px-4 -mb-px text-sm text-center text-gray-700 bg-transparent border-b-2 border-transparent sm:text-base dark:text-white whitespace-nowrap cursor-base focus:outline-none hover:border-gray-400">
            My courses
          </NavLink>
        </div>
        <Outlet />
      </div>
    </>
  )
}

export default Profile
