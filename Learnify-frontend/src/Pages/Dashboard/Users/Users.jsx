import HelmetHandler from "./../../../utils/HelmetHandler"
import TableUsers from "./TableUsers"

const Users = () => {

  return (
    <>
      <HelmetHandler title="Users" />

      <div className='p-5 bg-gray-900 px-10 min-h-screen text-white '>
        <h1 className='text-2xl'>Users</h1>
        <TableUsers />
      </div>
    </>

  )
}

export default Users