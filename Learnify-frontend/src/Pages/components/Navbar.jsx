import { useEffect, useState } from "react"
import { IoMdLogIn } from "react-icons/io"
import { RiUserAddLine } from "react-icons/ri"
import { Link } from "react-router-dom"
import { useDispatch, useSelector } from "react-redux"
import { logoutUser } from "../../redux/apiCalls/authApiCall"
import { getProfilePicture } from "../../redux/apiCalls/profileApiCall";
import { selectIsProfileImageLoading } from './../../redux/slices/profileSlice';

const Navbar = () => {
    const dispatch = useDispatch();
    const [show, setShow] = useState(false)
    const [dropped, setDropped] = useState(false)
    const { user } = useSelector(state => state.auth)
    const { profileImage } = useSelector(state => state.profile);
    const { isProfileImageLoading } = useSelector(selectIsProfileImageLoading);


    useEffect(() => {
        if (user && user.profilePictureId) {
            dispatch(getProfilePicture());
        }
    }, [dispatch, user]);

    const handleMenu = () => { setShow(!show) }
    const showDrop = () => { setDropped(!dropped) }

    return (
        <header className="bg-white relative border-b-2">
            <div className="mx-auto my-3 py-5 flex h-16 max-w-screen-xl items-center justify-between gap-8 px-4 sm:px-6 lg:px-8">
                <Link to="/">
                    <p className="text-2xl font-bold text-blue-600">Learnify</p>
                </Link>

                <div className="flex flex-1 items-center justify-end md:justify-end">
                    <div className="flex items-center gap-4">
                        {
                            user ? <div className="relative inline-block ">
                                {!user.profilePictureId ?
                                    (<img
                                        onClick={showDrop}
                                        className="object-cover cursor-pointer w-10 h-10 rounded-full ring ring-gray-300 dark:ring-gray-600"
                                        src="src/assets/fa-user-circle-icon.png"
                                        alt="Profile" />) :
                                    isProfileImageLoading ?
                                        (<img
                                            onClick={showDrop}
                                            className="object-cover cursor-pointer w-10 h-10 rounded-full ring ring-gray-300 dark:ring-gray-600"
                                            src="src/assets/fa-user-circle-icon.png"
                                            alt="Profile" />) :
                                        (<img
                                            onClick={showDrop}
                                            className="object-cover cursor-pointer w-10 h-10 rounded-full ring ring-gray-300 dark:ring-gray-600"
                                            src={profileImage}
                                            alt="Profile" />)
                                }
                                <div className={dropped ? "absolute right-0 z-20 w-48 py-2 mt-2 origin-top-right bg-gray-100 rounded-md shadow-xl dark:bg-gray-800" : "hidden"}>
                                    {
                                        user?.role === "Admin" ?
                                            <Link onClick={() => { setDropped(!dropped) }} to="/dashboard" className="flex items-center px-3 py-3 text-sm text-gray-600 capitalize transition-colors duration-300 transform dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-700 dark:hover:text-white">
                                                <svg className="w-5 h-5 mx-1" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                    <path d="M7 8C7 5.23858 9.23858 3 12 3C14.7614 3 17 5.23858 17 8C17 10.7614 14.7614 13 12 13C9.23858 13 7 10.7614 7 8ZM12 11C13.6569 11 15 9.65685 15 8C15 6.34315 13.6569 5 12 5C10.3431 5 9 6.34315 9 8C9 9.65685 10.3431 11 12 11Z" fill="currentColor"></path>
                                                    <path d="M6.34315 16.3431C4.84285 17.8434 4 19.8783 4 22H6C6 20.4087 6.63214 18.8826 7.75736 17.7574C8.88258 16.6321 10.4087 16 12 16C13.5913 16 15.1174 16.6321 16.2426 17.7574C17.3679 18.8826 18 20.4087 18 22H20C20 19.8783 19.1571 17.8434 17.6569 16.3431C16.1566 14.8429 14.1217 14 12 14C9.87827 14 7.84344 14.8429 6.34315 16.3431Z" fill="currentColor"></path>
                                                </svg>

                                                <span className="mx-1">
                                                    Dashboard
                                                </span>
                                            </Link>
                                            :
                                            <Link onClick={() => { setDropped(!dropped) }} to="/me/profile" className="flex items-center px-3 py-3 text-sm text-gray-600 capitalize transition-colors duration-300 transform dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-700 dark:hover:text-white">
                                                <svg className="w-5 h-5 mx-1" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                    <path d="M7 8C7 5.23858 9.23858 3 12 3C14.7614 3 17 5.23858 17 8C17 10.7614 14.7614 13 12 13C9.23858 13 7 10.7614 7 8ZM12 11C13.6569 11 15 9.65685 15 8C15 6.34315 13.6569 5 12 5C10.3431 5 9 6.34315 9 8C9 9.65685 10.3431 11 12 11Z" fill="currentColor"></path>
                                                    <path d="M6.34315 16.3431C4.84285 17.8434 4 19.8783 4 22H6C6 20.4087 6.63214 18.8826 7.75736 17.7574C8.88258 16.6321 10.4087 16 12 16C13.5913 16 15.1174 16.6321 16.2426 17.7574C17.3679 18.8826 18 20.4087 18 22H20C20 19.8783 19.1571 17.8434 17.6569 16.3431C16.1566 14.8429 14.1217 14 12 14C9.87827 14 7.84344 14.8429 6.34315 16.3431Z" fill="currentColor"></path>
                                                </svg>

                                                <span className="mx-1">
                                                    My account
                                                </span>
                                            </Link>

                                    }

                                    <span onClick={() => {
                                        dispatch(logoutUser());
                                        setDropped(!dropped);
                                    }} className="flex cursor-pointer items-center p-3 text-sm text-gray-600 capitalize transition-colors duration-300 transform dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-700 dark:hover:text-white">
                                        <svg className="w-5 h-5 mx-1" xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 16 16">
                                            <path fillRule="evenodd" d="M10 12.5a.5.5 0 0 1-.5.5h-8a.5.5 0 0 1-.5-.5v-9a.5.5 0 0 1 .5-.5h8a.5.5 0 0 1 .5.5v2a.5.5 0 0 0 1 0v-2A1.5 1.5 0 0 0 9.5 2h-8A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h8a1.5 1.5 0 0 0 1.5-1.5v-2a.5.5 0 0 0-1 0z" />
                                            <path fillRule="evenodd" d="M15.854 8.354a.5.5 0 0 0 0-.708l-3-3a.5.5 0 0 0-.708.708L14.293 7.5H5.5a.5.5 0 0 0 0 1h8.793l-2.147 2.146a.5.5 0 0 0 .708.708z" />
                                        </svg>

                                        <span className="mx-1">
                                            Logout
                                        </span>
                                    </span>

                                </div>
                            </div> :
                                <>
                                    <div className={show ? "dropNav md:hidden" : "hidden md:flex  sm:gap-4"} >
                                        <Link onClick={() => setShow(!show)} to="/login" className="bg-white transition-all flex items-center text-gray-700 dark:text-gray-300 justify-center gap-x-2 text-xs sm:text-base  dark:bg-gray-900 dark:border-gray-700 dark:hover:bg-gray-800 rounded-lg hover:bg-gray-100 duration-300 border px-5 py-2.5">
                                            <IoMdLogIn className='text-yellow-600 text-2xl' />
                                            <span className='text-sm'>Login</span>
                                        </Link>

                                        <Link onClick={() => setShow(!show)} to="/register" className="bg-rose-500 border-2 border-rose-500 hover:text-rose-500 hover:bg-white flex items-center text-white dark:text-gray-300 justify-center gap-x-3 text-sm sm:text-base  dark:bg-gray-900 dark:border-gray-700 dark:hover:bg-gray-800 rounded-lg duration-300 transition-colors px-5 py-2.5">
                                            <RiUserAddLine className='text-2xl' />
                                            <span className='text-sm'>Sign up</span>
                                        </Link>
                                    </div>
                                    <button onClick={handleMenu}
                                        className="block rounded bg-gray-100 p-2.5 text-gray-600 transition hover:text-gray-600/75 md:hidden">
                                        {
                                            show ?
                                                <svg
                                                    xmlns="http://www.w3.org/2000/svg"
                                                    width="20"
                                                    height="20"
                                                    fill="currentColor"
                                                    className="bi bi-x" viewBox="0 0 16 16">
                                                    <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708" />
                                                </svg>
                                                :
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
                                        }
                                    </button>
                                </>
                        }
                    </div>
                </div>
            </div>
        </header >
    )
}

export default Navbar
