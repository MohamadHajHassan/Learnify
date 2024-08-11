import { useState } from "react";
import "./Sidebar.css"
import { BsDatabaseFillGear } from 'react-icons/bs';
import { FaPhotoVideo } from 'react-icons/fa';
import { IoMdLogOut } from 'react-icons/io';
import { IoHome } from 'react-icons/io5';
import { LuUsers } from 'react-icons/lu';
import { useDispatch } from 'react-redux';
import { Link, NavLink } from 'react-router-dom';
import { logoutUser } from "../../../redux/apiCalls/authApiCall"

const Sidebar = () => {
    const [show, setShow] = useState(false);
    const dispatch = useDispatch();

    const NavLinks = [
        { name: 'Courses', link: '/dashboard/courses', icon: <FaPhotoVideo /> },
        { name: 'Users', link: '/dashboard/users', icon: <LuUsers /> },
    ];

    return (
        <>
            <div>
                <div className="sidebar border-l fixed top-0 bottom-0 -left-[300px] lg:left-0 p-2 w-[300px] overflow-y-auto text-center bg-gray-900 border-r-2 border-r-slate-600">
                    <div className="text-gray-100 text-xl">
                        <div className="p-2.5 mt-1 flex items-center">
                            <i className="bi bi-amd px-4 py-3 rounded-md bg-indigo-500">
                                <BsDatabaseFillGear />
                            </i>
                            <h1 className="font-bold text-gray-200 text-[20px] ml-3">Dashboard</h1>
                        </div>
                        <div className="my-2 bg-gray-600 h-[1px]"></div>
                    </div>
                    <Link to='/' className='p-2.5 NavSide mt-3 flex items-center rounded-md px-4 duration-300 cursor-pointer hover:bg-indigo-500 text-white'>
                        <IoHome />
                        <span className='text-[15px] ml-4 text-gray-200 font-bold'>Home</span>
                    </Link>
                    {
                        NavLinks.map(({ name, link, icon }) => (
                            <NavLink to={link}
                                className="p-2.5 NavSide mt-3 flex items-center rounded-md px-4 duration-300 cursor-pointer hover:bg-indigo-500 text-white"
                                key={link}
                            >
                                {icon}
                                <span className="text-[15px] ml-4 text-gray-200 font-bold">{name}</span>
                            </NavLink>

                        )
                        )
                    }
                    <button onClick={() => { dispatch(logoutUser()) }} className='p-2.5 w-full NavSide mt-3 flex items-center rounded-md px-4 duration-300 cursor-pointer hover:bg-indigo-500 text-white'>
                        <IoMdLogOut />
                        <span className='text-[15px] ml-4 text-gray-200 font-bold'>Logout</span>
                    </button>
                </div>
            </div>
            <div className="fixed z-50 top-0 left-0 right-0 block lg:hidden">
                <header className="bg-gray-900 border-b">
                    <div className="mx-auto flex h-14 max-w-screen-xl items-center gap-8 px-4 sm:px-6 lg:px-8">
                        <div className="p-2.5 mt-1 flex items-center">
                            <i className="bi bi-amd px-4 py-3 text-white rounded-md bg-indigo-500">
                                <BsDatabaseFillGear />
                            </i>
                            <h1 className="font-bold text-gray-200 text-[20px] ml-3">Dashboard</h1>
                        </div>

                        <div className="flex flex-1 items-center justify-end">
                            <div className="flex items-center gap-4">
                                <div className="sm:flex sm:gap-4">
                                    <button
                                        onClick={() => setShow(!show)}
                                        className="block rounded px-4 bg-gray-800 p-2.5 text-gray-100 transition hover:text-gray-300/75 lg:hidden"
                                    >
                                        <span className="sr-only">Toggle menu</span>
                                        <svg
                                            xmlns="http://www.w3.org/2000/svg"
                                            className="h-5 w-5"
                                            fill="none"
                                            viewBox="0 0 24 24"
                                            stroke="currentColor"
                                            strokeWidth="2"
                                        >
                                            <path strokeLinecap="round" strokeLinejoin="round" d="M4 6h16M4 12h16M4 18h16" />
                                        </svg>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </header>
                <div className={show ? "navbar-links gap-5 border-b active" : "navbar-links gap-5 border-b"}>
                    <Link to='/' className='p-2.5 NavSide mt-3 flex items-center rounded-md px-4 duration-300 cursor-pointer hover:bg-indigo-500 text-white'>
                        <IoHome />
                        <span className='text-[15px] ml-4 text-gray-200 font-bold'>Home</span>
                    </Link>
                    {
                        NavLinks.map(({ name, link, icon }) => (
                            <NavLink to={link}
                                className="p-2.5 NavSide mt-3 flex items-center rounded-md px-4 duration-300 cursor-pointer hover:bg-indigo-500 text-white"
                                key={link}
                                onClick={() => setShow(!show)}
                            >
                                {icon}
                                <span className="text-[15px] ml-4 text-gray-200 font-bold">{name}</span>
                            </NavLink>
                        ))
                    }
                    <button onClick={() => { dispatch(logoutUser()) }} className='p-2.5 w-full NavSide mt-3 flex items-center rounded-md px-4 duration-300 cursor-pointer hover:bg-indigo-500 text-white'>
                        <IoMdLogOut />
                        <span className='text-[15px] ml-4 text-gray-200 font-bold'>Logout</span>
                    </button>
                </div>
            </div>
        </>
    )
}

export default Sidebar