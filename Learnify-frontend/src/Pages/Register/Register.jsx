import { useEffect, useState } from 'react'
import HelmetHandler from '../../utils/HelmetHandler'
import { useDispatch } from 'react-redux';
import { Link } from 'react-router-dom'
import { toast } from 'react-toastify';
import { registerUser } from '../../redux/apiCalls/authApiCall';

const Register = () => {
  const [fName, setFname] = useState("");
  const [lName, setLname] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const dispatch = useDispatch();

  const submitHandler = (e) => {
    e.preventDefault();
    if (fName.trim() === "" || lName.trim() === ""
      || email.trim() === "" || password.trim() === ""
      || confirmPassword.trim() === "") {
      return toast.error("Please fill in all fields")
    } else if (password !== confirmPassword) {
      return toast.error("Passwords do not match")
    }
    dispatch(registerUser({ fName, lName, email, password, confirmPassword }));
    setFname("");
    setLname("");
    setEmail("");
    setPassword("");
    setConfirmPassword("");
  }

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  return (
    <>
      <HelmetHandler title="Create an account" />
      <div className='bg-slate-100 py-16'>
        <section className="max-w-xl p-6 mx-auto bg-white rounded-md shadow-md">
          <h2 className="text-lg font-semibold text-gray-700 capitalize dark:text-white">Signup</h2>
          <form onSubmit={submitHandler}>
            <div className="grid grid-cols-1 gap-6 mt-4 md:grid-cols-2">
              <div className="relative before:absolute before:bottom-0 before:h-0.5 before:left-0 before:origin-right focus-within:before:origin-left before:right-0 before:scale-x-0 before:m-auto before:bg-sky-400 dark:before:bg-sky-800 focus-within:before:!scale-x-100 focus-within:invalid:before:bg-red-400 before:transition before:duration-300">
                <input
                  value={fName}
                  onChange={(e) => { setFname(e.target.value) }}
                  type="text"
                  placeholder="First Name"
                  className="w-full bg-transparent pb-3  border-b border-gray-300 dark:placeholder-gray-300 dark:border-gray-600 outline-none  invalid:border-red-400 transition" />
              </div>
              <div className="relative before:absolute before:bottom-0 before:h-0.5 before:left-0 before:origin-right focus-within:before:origin-left before:right-0 before:scale-x-0 before:m-auto before:bg-sky-400 dark:before:bg-sky-800 focus-within:before:!scale-x-100 focus-within:invalid:before:bg-red-400 before:transition before:duration-300">
                <input
                  value={lName}
                  onChange={(e) => { setLname(e.target.value) }}
                  type="text"
                  placeholder="Last Name"
                  className="w-full bg-transparent pb-3  border-b border-gray-300 dark:placeholder-gray-300 dark:border-gray-600 outline-none  invalid:border-red-400 transition" />
              </div>
            </div>
            <div className="relative mt-5 before:absolute before:bottom-0 before:h-0.5 before:left-0 before:origin-right focus-within:before:origin-left before:right-0 before:scale-x-0 before:m-auto before:bg-sky-400 dark:before:bg-sky-800 focus-within:before:!scale-x-100 focus-within:invalid:before:bg-red-400 before:transition before:duration-300">
              <input
                value={email}
                onChange={(e) => { setEmail(e.target.value) }}
                type="email"
                placeholder="Email"
                className="w-full bg-transparent pb-3  border-b border-gray-300 dark:placeholder-gray-300 dark:border-gray-600 outline-none  invalid:border-red-400 transition" />
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="relative mt-5 before:absolute before:bottom-0 before:h-0.5 before:left-0 before:origin-right focus-within:before:origin-left before:right-0 before:scale-x-0 before:m-auto before:bg-sky-400 dark:before:bg-sky-800 focus-within:before:!scale-x-100 focus-within:invalid:before:bg-red-400 before:transition before:duration-300">
                <input
                  value={password}
                  onChange={(e) => { setPassword(e.target.value) }}
                  type="password"
                  placeholder="Password"
                  className="w-full bg-transparent pb-3  border-b border-gray-300 dark:placeholder-gray-300 dark:border-gray-600 outline-none  invalid:border-red-400 transition" />
              </div>
              <div className="relative mt-5 before:absolute before:bottom-0 before:h-0.5 before:left-0 before:origin-right focus-within:before:origin-left before:right-0 before:scale-x-0 before:m-auto before:bg-sky-400 dark:before:bg-sky-800 focus-within:before:!scale-x-100 focus-within:invalid:before:bg-red-400 before:transition before:duration-300">
                <input
                  value={confirmPassword}
                  onChange={(e) => { setConfirmPassword(e.target.value) }}
                  type="password"
                  placeholder="Confirm Password"
                  className="w-full bg-transparent pb-3  border-b border-gray-300 dark:placeholder-gray-300 dark:border-gray-600 outline-none  invalid:border-red-400 transition" />
              </div>
            </div>

            <div className="flex justify-end mt-6">
              <button type='submit' className="px-8 py-2.5 leading-5 text-white transition-colors duration-300 transform border-gray-700 border-2 hover:text-gray-700 bg-gray-700 rounded-md hover:bg-white focus:outline-none focus:bg-gray-600">Create an account</button>
            </div>
            <div className="flex justify-right mt-6">
              <p>Already a user? <Link to="/login" className='underline text-yellow-500 text-right'>Login</Link></p>
            </div>

          </form>
        </section>
      </div>
    </>
  )
}

export default Register
