/* eslint-disable react/prop-types */
import { Dialog, DialogPanel, DialogTitle, Transition, TransitionChild, Input, Field, Label, Description, Select } from '@headlessui/react'
import { Fragment, useState } from 'react'
import request from "../../../utils/request"
import { toast } from 'react-toastify'

const RowUser = ({ data, fetchUsers }) => {
    const [id] = useState(data.id);
    const [fName, setFname] = useState(data.firstName);
    const [lName, setLname] = useState(data.lastName);
    const [email] = useState(data.email);
    const [role, setRole] = useState(data.role);
    const [isEmailVerified] = useState(data.isEmailConfirmed);
    const [profilePictureId] = useState(data.profilePictureId);
    const [createdOn] = useState(data.createdOn);
    const [isOpenDelete, setIsOpenDelete] = useState(false);
    const [isOpenEdit, setIsOpenEdit] = useState(false);

    const deleteUserHandler = async () => {
        try {
            await request.delete(`/Users/${id}`)
            fetchUsers()
            toast.success("Deleted Successfully!")
        } catch (error) {
            console.error(error)
        } finally {
            setIsOpenDelete(false)
        }
    }

    const editUserHandler = async () => {
        try {
            const formData = new FormData();
            formData.append("FirstName", fName);
            formData.append("LastName", lName);

            const config = {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            };

            await request.put(`/Users/${id}`, formData, config)

            if (role === "Admin" && data.role !== "Admin") {
                await request.put(`/Users/setAdmin/${id}`)
            } else if (role === "Student" && data.role === "Admin") {
                await request.put(`/Users/setStudent/${id}`)
            }
            toast.success("Updated Successfully!")
            fetchUsers()
        } catch (error) {
            console.error(error)
        } finally {
            setIsOpenEdit(false)
        }
    }

    return (
        <Fragment>
            <tr>
                <td className="whitespace-nowrap px-4 py-2 font-medium text-white">{id}</td>
                <td className="whitespace-nowrap px-4 py-2 font-medium text-white">{fName}</td>
                <td className="whitespace-nowrap px-4 py-2 font-medium text-white">{lName}</td>
                <td className="whitespace-nowrap px-4 py-2 text-white">{email}</td>
                <td className="whitespace-nowrap px-4 py-2 text-white">
                    {isEmailVerified ? "Yes" : "No"}
                </td>
                <td className="whitespace-nowrap px-4 py-2 text-white">
                    {data.role === "Admin" ? "Admin" : "Student"}
                </td>
                <td className="whitespace-nowrap px-4 py-2 text-white">{profilePictureId}</td>
                <td className="whitespace-nowrap px-4 py-2 text-white">{createdOn}</td>
                <td className="whitespace-nowrap px-4 py-2 text-white">
                    <button
                        onClick={() => { setIsOpenEdit(true) }}
                        className='inline-block rounded bg-indigo-600 px-5 py-2 mx-2 text-sm font-medium text-white transition hover:scale-105'>
                        Edit
                    </button>
                    <button
                        onClick={() => { setIsOpenDelete(true) }}
                        className='inline-block rounded bg-red-600 px-5 py-2 mx-2 text-sm font-medium text-white transition hover:scale-105'>
                        Delete
                    </button>
                </td>
            </tr>

            <Transition appear show={isOpenDelete}>
                <Dialog
                    as="div"
                    className="relative z-10 focus:outline-none"
                    onClose={() => { setIsOpenDelete(false) }}
                >
                    <div className="fixed inset-0 z-10 w-screen overflow-y-auto">
                        <div className="flex min-h-full items-center justify-center p-4">
                            <TransitionChild
                                enter="ease-out duration-300"
                                enterFrom="opacity-0 transform-[scale(95%)]"
                                enterTo="opacity-100 transform-[scale(100%)]"
                                leave="ease-in duration-200"
                                leaveFrom="opacity-100 transform-[scale(100%)]"
                                leaveTo="opacity-0 transform-[scale(95%)]"
                            >
                                <DialogPanel className="w-full max-w-lg rounded-xl shadow-2xl border border-white bg-gray-900 p-6">
                                    <DialogTitle as="h1" className="font-bold text-xl mb-5 text-white">
                                        Are you sure you want to delete this user?
                                    </DialogTitle>
                                    <Description className="text-gray-200">
                                        If you delete the user, you can&apos;t get it back. If you are want to proceed click yes
                                    </Description>
                                    <div className="mt-7 grid grid-cols-1 md:grid-cols-2 gap-2 md:gap-5">
                                        <button
                                            onClick={deleteUserHandler}
                                            className='inline-block rounded bg-red-600 px-8 py-3 text-sm font-medium text-white transition hover:scale-105'>
                                            Delete
                                        </button>
                                        <button
                                            onClick={() => { setIsOpenDelete(false) }}
                                            className='inline-block rounded bg-indigo-600 px-8 py-3 text-sm font-medium text-white transition hover:scale-105'>
                                            Cancel
                                        </button>
                                    </div>
                                </DialogPanel>
                            </TransitionChild>
                        </div>
                    </div>
                </Dialog>
            </Transition>

            <Transition appear show={isOpenEdit}>
                <Dialog as="div" className="relative z-10 focus:outline-none" onClose={() => { setIsOpenEdit(false) }}>
                    <div className="fixed inset-0 z-10 w-screen overflow-y-auto">
                        <div className="flex min-h-full items-center justify-center p-4">
                            <TransitionChild
                                enter="ease-out duration-300"
                                enterFrom="opacity-0 transform-[scale(95%)]"
                                enterTo="opacity-100 transform-[scale(100%)]"
                                leave="ease-in duration-200"
                                leaveFrom="opacity-100 transform-[scale(100%)]"
                                leaveTo="opacity-0 transform-[scale(95%)]"
                            >
                                <DialogPanel className="w-full max-w-lg rounded-xl shadow-2xl border border-white bg-gray-900 p-6">
                                    <DialogTitle as="h1" className="font-bold text-xl mb-5 text-white">
                                        Edit User
                                    </DialogTitle>

                                    <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                                        <Field>
                                            <Label className="text-sm/6 mr-1 font-medium text-white">First Name</Label>
                                            <Input value={fName} onChange={(e) => { setFname(e.target.value) }}
                                                className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                                            />
                                        </Field>
                                        <Field >
                                            <Label className="text-sm/6 font-medium text-white">Last Name</Label>
                                            <Input value={lName} onChange={(e) => { setLname(e.target.value) }}
                                                className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                                            />
                                        </Field>
                                    </div>
                                    <div className="grid mt-3 grid-cols-1 md:grid-cols-2 gap-3">
                                        <Field>
                                            <Label className="text-sm/6 font-medium text-white">Role</Label>
                                            <Select value={role} onChange={(e) => { setRole(e.target.value) }}
                                                className="mt-2 block w-full rounded-lg border-none bg-white/5 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25"
                                            >
                                                <option className='text-black' value="Student">Student</option>
                                                <option className='text-black' value="Admin">Admin</option>
                                            </Select>
                                        </Field>
                                    </div>


                                    <div className="mt-7 grid grid-cols-1 md:grid-cols-2 gap-2 md:gap-5">
                                        <button
                                            onClick={editUserHandler}
                                            className='inline-block rounded bg-indigo-600 px-8 py-3 text-sm font-medium text-white transition hover:scale-105'>
                                            Save
                                        </button>
                                        <button
                                            onClick={() => { setIsOpenEdit(false) }}
                                            className='inline-block rounded bg-indigo-600 px-8 py-3 text-sm font-medium text-white transition hover:scale-105'>
                                            Cancel
                                        </button>
                                    </div>
                                </DialogPanel>
                            </TransitionChild>
                        </div>
                    </div>
                </Dialog>
            </Transition>

        </Fragment>
    )
}

export default RowUser