import { useEffect, useState } from 'react';
import RowUser from './RowUser'
import request from '../../../utils/request';

const TableUsers = () => {
    const [users, setUsers] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    const fetchUsers = async () => {
        try {
            const { data } = await request.get("/Users");
            setUsers(data);
        } catch (error) {
            console.error("Error fetching users:", error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchUsers();
    }, []);

    return (
        <>
            <div className="overflow-x-auto mt-5 table-users text-left border-spacing-x-5 rounded-lg border border-gray-200">
                <table className="min-w-full divide-y-2 divide-gray-200 text-white text-sm">
                    <thead>
                        <tr>
                            <th className="whitespace-nowrap px-4 py-2 font-medium text-white">Id</th>
                            <th className="whitespace-nowrap px-4 py-2 font-medium text-white">First Name</th>
                            <th className="whitespace-nowrap px-4 py-2 font-medium text-white">Last Name</th>
                            <th className="whitespace-nowrap px-4 py-2 font-medium text-white">Email</th>
                            <th className="whitespace-nowrap px-4 py-2 font-medium text-white">Is Email Verified</th>
                            <th className="whitespace-nowrap px-4 py-2 font-medium text-white">Role</th>
                            <th className="whitespace-nowrap px-4 py-2 font-medium text-white">Profile Picture Id</th>
                            <th className="whitespace-nowrap px-4 py-2 font-medium text-white">Created On</th>
                        </tr>
                    </thead>

                    <tbody className="divide-y table-users divide-gray-200">
                        {isLoading ? (
                            <tr>
                                <td colSpan="7" className="text-center py-4">Loading...</td>
                            </tr>
                        ) : (
                            users.map((user) => {
                                return <RowUser
                                    data={user}
                                    fetchUsers={fetchUsers}
                                    key={user.id} />
                            })
                        )}
                    </tbody>
                </table>
            </div>
        </>
    )
}

export default TableUsers