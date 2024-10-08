import { useEffect, useState } from 'react'
import HelmetHandler from '../../utils/HelmetHandler';
import { useDispatch } from 'react-redux';
import { Link } from 'react-router-dom'
import { loginUser } from '../../redux/apiCalls/authApiCall';
import { toast } from 'react-toastify';

const Login = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const dispatch = useDispatch();

    const submitHandler = (e) => {
        e.preventDefault();
        if (email.trim() === "" || password.trim() === "") {
            return toast.error("Please fill in all fields")
        }
        dispatch(loginUser({ email, password }))
    }

    useEffect(() => {
        window.scrollTo(0, 0);
    }, []);

    return (
        <>
            <HelmetHandler title="Login" />
            <div className='bg-slate-100 py-16'>
                <section className="max-w-xl p-6 mx-auto bg-white rounded-md shadow-md">
                    <h2 className="text-lg font-semibold text-gray-700 capitalize dark:text-white">Login</h2>

                    <form onSubmit={submitHandler}>
                        <div className="grid grid-cols-1 gap-4 mt-4">
                            <div className="relative mt-5 before:absolute before:bottom-0 before:h-0.5 before:left-0 before:origin-right focus-within:before:origin-left before:right-0 before:scale-x-0 before:m-auto before:bg-sky-400 dark:before:bg-sky-800 focus-within:before:!scale-x-100 focus-within:invalid:before:bg-red-400 before:transition before:duration-300">
                                <input
                                    value={email}
                                    onChange={(e) => { setEmail(e.target.value) }}
                                    type="email"
                                    placeholder="Email"
                                    className="w-full bg-transparent pb-3  border-b border-gray-300 dark:placeholder-gray-300 dark:border-gray-600 outline-none  invalid:border-red-400 transition" />
                            </div>
                            <div className="relative mt-3 mb-5 before:absolute before:bottom-0 before:h-0.5 before:left-0 before:origin-right focus-within:before:origin-left before:right-0 before:scale-x-0 before:m-auto before:bg-sky-400 dark:before:bg-sky-800 focus-within:before:!scale-x-100 focus-within:invalid:before:bg-red-400 before:transition before:duration-300">
                                <input
                                    value={password}
                                    onChange={(e) => { setPassword(e.target.value) }}
                                    type="password"
                                    placeholder="Password"
                                    className="w-full bg-transparent pb-3  border-b border-gray-300 dark:placeholder-gray-300 dark:border-gray-600 outline-none  invalid:border-red-400 transition" />
                            </div>
                        </div>

                        <div className="flex justify-end mt-6">
                            <button className="px-8 py-2.5 leading-5 text-white transition-colors duration-300 transform border-gray-700 border-2 hover:text-gray-700 bg-gray-700 rounded-md hover:bg-white focus:outline-none focus:bg-gray-600">Login</button>
                        </div>
                        <div className="flex justify-right mt-6">
                            <p>You do not have an account yet?
                                <Link to="/register"
                                    className='underline text-yellow-500 text-right'> Signup
                                </Link>
                            </p>
                        </div>
                    </form>
                </section>

            </div>
        </>
    )
}

export default Login
