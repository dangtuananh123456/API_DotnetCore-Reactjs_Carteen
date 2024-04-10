import { toast } from "react-toastify";
import { useEffect, useState } from "react";
import { useDropzone } from "react-dropzone";
import AddIcon from "@mui/icons-material/Add";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import ImageIcon from "@mui/icons-material/Image";
import { Button, IconButton, Tooltip } from "@mui/material";
import {
    useLazyGetAllProductQuery,
    useAddProductMutation,
    useModifyProductMutation,
    useDeleteProductMutation,
} from "../../services/product";
import { uploadToCloudinary } from "../../utils/uploadToCloudinary";
import AdminTable from "../../components/Table/Table";
import ProductFormDialog from "../../components/Dialog/FormDialog";
import { useGetAllCategoryQuery } from "../../services/category";
import Toast from "../../components/Toast/Toast";

const MenuPage = () => {
    const [getAllProduct, { data: products }] = useLazyGetAllProductQuery();
    const { data: categories } = useGetAllCategoryQuery();
    const [addProduct, { isSuccess: addProductSuccess }] =
        useAddProductMutation();
    const [modifyProduct, { isSuccess: modifyProductSuccess }] =
        useModifyProductMutation();
    const [deleteProduct, { isSuccess: deleteProductSuccess }] =
        useDeleteProductMutation();
    const [isLoading, setIsLoading] = useState(false);
    const [open, setOpen] = useState(false);
    const [product, setProduct] = useState(null);
    const [editingProductId, setEditingProductId] = useState(null);
    const [imagesFile, setImagesFile] = useState(null);
    const [thumbnailFile, setThumbnailFile] = useState(null);

    function handleOpen() {
        setOpen(true);
    }
    function handleClose() {
        setOpen(false);
        setEditingProductId(null);
        setProduct(null);
        setImagesFile(null);
        setThumbnailFile(null);
    }
    const handleProductImage = (e) => {
        setImagesFile(null);
        setProduct((prev) => ({ ...prev, images: null }));
        const fileUpload = e.target.files;
        if (fileUpload.length > 5) {
            return;
        }
        for (const file of fileUpload) {
            setImagesFile((prev) => {
                let temp = prev;
                if (!temp) {
                    temp = new Array();
                }
                temp.push(file);
                return temp;
            });
            const image = URL.createObjectURL(file);
            setProduct((prev) => {
                const temp = { ...prev };
                if (!temp.images) {
                    temp.images = new Array();
                } else {
                    temp.images = JSON.parse(temp.images);
                }
                temp.images.push(image);
                temp.images = JSON.stringify(temp.images);
                return temp;
            });
        }
    };
    const handleInputChange = (e) => {
        setProduct((prev) => ({ ...prev, [e.target.id]: e.target.value }));
    };

    const onDrop = (acceptedFiles) => {
        const file = acceptedFiles[0];
        setThumbnailFile(file);
        const imageUrl = URL.createObjectURL(file);
        setProduct((prev) => ({ ...prev, thumbnail: imageUrl }));
    };

    const { getRootProps, getInputProps } = useDropzone({
        accept: {
            "image/*": [".jpeg", ".jpg", ".png"],
        },
        onDrop,
    });

    const handleEdit = (productId) => {
        setOpen(true);
        setEditingProductId(productId);
        const productToEdit = products?.data.find(
            (product) => product.id === productId
        );
        setProduct({ ...productToEdit });
    };
    const handleDelete = (productId) => {
        deleteProduct({ id: productId });
    };
    const handleSubmit = async (e) => {
        e.preventDefault();
        if (
            product?.name &&
            product?.description &&
            product?.thumbnail &&
            product?.price &&
            product?.discount &&
            product?.stock &&
            product?.categoryId
        ) {
            if (editingProductId) {
                let thumbnailUrl = null;
                let images = null;
                setIsLoading(true);
                if (thumbnailFile) {
                    const res = await uploadToCloudinary(
                        thumbnailFile,
                        "canteen/product"
                    );
                    const { secure_url } = res.data;
                    thumbnailUrl = secure_url;
                }
                if (imagesFile) {
                    images = new Array();
                    for (const imageFile of imagesFile) {
                        const res = await uploadToCloudinary(
                            imageFile,
                            "canteen/product"
                        );
                        const { secure_url } = res.data;
                        images.push(secure_url);
                    }
                }
                if (thumbnailUrl && images) {
                    modifyProduct({
                        ...product,
                        thumbnail: thumbnailUrl,
                        images: JSON.stringify(images),
                    });
                } else if (thumbnailUrl) {
                    modifyProduct({
                        ...product,
                        thumbnail: thumbnailUrl,
                    });
                } else if (images) {
                    modifyProduct({
                        ...product,
                        images: JSON.stringify(images),
                    });
                } else {
                    modifyProduct({
                        ...product,
                    });
                }
            } else {
                let images = null;
                setIsLoading(true);
                const res = await uploadToCloudinary(
                    thumbnailFile,
                    "canteen/product"
                );
                const { secure_url } = res.data;
                if (imagesFile) {
                    images = new Array();
                    for (const imageFile of imagesFile) {
                        const res = await uploadToCloudinary(
                            imageFile,
                            "canteen/product"
                        );
                        const { secure_url } = res.data;
                        images.push(secure_url);
                    }
                }
                if (images) {
                    addProduct({
                        ...product,
                        thumbnail: secure_url,
                        images: JSON.stringify(images),
                    });
                } else {
                    addProduct({
                        ...product,
                        thumbnail: secure_url,
                    });
                }
            }
        }
    };

    useEffect(() => {
        getAllProduct();
    }, [getAllProduct]);
    useEffect(() => {
        if (deleteProductSuccess) {
            toast.success("Delete successfully", {
                position: toast.POSITION.BOTTOM_RIGHT,
            });
            setTimeout(() => {
                getAllProduct();
            }, 500);
        }
    }, [deleteProductSuccess, getAllProduct]);
    useEffect(() => {
        if (modifyProductSuccess) {
            setIsLoading(false);
            handleClose();
            toast.success("Edit successfully", {
                position: toast.POSITION.BOTTOM_RIGHT,
            });
            setTimeout(() => {
                getAllProduct();
            }, 500);
        }
    }, [modifyProductSuccess, getAllProduct]);
    useEffect(() => {
        if (addProductSuccess) {
            setIsLoading(false);
            handleClose();
            toast.success("Add successfully", {
                position: toast.POSITION.BOTTOM_RIGHT,
            });
            setTimeout(() => {
                getAllProduct();
            }, 500);
        }
    }, [addProductSuccess, getAllProduct]);

    const columns = [
        { field: "id", headerName: "ID", width: 50 },
        {
            field: "thumbnail",
            headerName: "Thumbnail",
            headerAlign: "center",
            align: "center",
            width: 150,
            sortable: false,
            renderCell: (rowData) => (
                <div
                    className="w-[50px] h-[50px] bg-center bg-cover rounded-md border-[1px] border-primary-light"
                    style={{
                        backgroundImage: `url(${rowData.formattedValue})`,
                    }}
                />
            ),
        },
        {
            field: "name",
            headerName: "Name",
            width: 200,
            sortable: false,
        },
        {
            field: "description",
            headerName: "Description",
            headerAlign: "center",
            width: 350,
            sortable: false,
        },
        {
            field: "price",
            headerName: "Price",
            headerAlign: "center",
            align: "center",
            type: "number",
            width: 100,
        },
        {
            field: "discount",
            headerName: "Discount",
            headerAlign: "center",
            align: "center",
            type: "number",
            width: 100,
        },
        // {
        //     field: "rating",
        //     headerName: "Rating",
        //     type: "number",
        //     width: 150,
        //     renderCell: (rowData) => (
        //         <div>
        //             {Array.from({ length: 5 }, (_, index) => index).map(
        //                 (index) => (
        //                     <i
        //                         key={index}
        //                         className={`${
        //                             index < rowData.formattedValue
        //                                 ? "fa-solid"
        //                                 : "fa-regular"
        //                         } fa-star text-yellow-500`}
        //                     />
        //                 )
        //             )}
        //             <span className="ml-2">{rowData.formattedValue}</span>
        //         </div>
        //     ),
        //     headerAlign: "center",
        //     align: "right",
        // },
        {
            field: "stock",
            headerName: "Stock",
            headerAlign: "center",
            align: "center",
            type: "number",
            width: 100,
        },
        {
            field: "actions",
            headerName: "Actions",
            headerAlign: "center",
            align: "center",
            width: 100,
            sortable: false,
            renderCell: (rowData) => (
                <div className="flex items-center justify-center gap-4">
                    <Tooltip title="Edit">
                        <IconButton onClick={() => handleEdit(rowData.row.id)}>
                            <EditIcon className="hover:text-green-500 transition-all" />
                        </IconButton>
                    </Tooltip>
                    <Tooltip title="Delete">
                        <IconButton
                            onClick={() => handleDelete(rowData.row.id)}
                        >
                            <DeleteIcon className="hover:text-red-500 transition-all" />
                        </IconButton>
                    </Tooltip>
                </div>
            ),
        },
    ];
    const rows = products?.data ?? [];
    console.log(categories?.data);

    return (
        <div className="p-5">
            <h1 className="text-3xl font-bold text-gray-700 w-fit border-b-[3px] border-primary-light">
                MENU
            </h1>
            <button
                onClick={() => handleOpen()}
                className="my-10 w-full max-w-fit flex items-center gap-x-2 text-white text-lg font-semibold uppercase rounded-md px-4 py-2 bg-primary transition-all duration-300 hover:bg-primary-light"
            >
                Add food
                <AddIcon />
            </button>
            <ProductFormDialog
                title={editingProductId ? "UPDATE FOOD" : "ADD FOOD"}
                open={open}
                onSetClose={handleClose}
            >
                <form onSubmit={handleSubmit}>
                    <div className="flex flex-col mb-4">
                        <label
                            htmlFor="categoryId"
                            className="text-sm font-semibold text-gray-600 mb-2"
                        >
                            Product Category
                        </label>
                        <select
                            name="categoryId"
                            id="categoryId"
                            className="border-[1px] outline-none p-2 rounded-md"
                            value={product?.categoryId}
                            onChange={(e) => handleInputChange(e)}
                        >
                            {categories?.data?.map((category, index) => (
                                <option value={category.id} key={index}>
                                    {category.name}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="flex flex-col mb-4">
                        <label
                            htmlFor="name"
                            className="text-sm font-semibold text-gray-600 mb-2"
                        >
                            Product Name
                        </label>
                        <input
                            id="name"
                            name="name"
                            className="border-none outline-none py-2 px-3 rounded-[4px] bg-gray-100"
                            type="text"
                            placeholder="Enter product name"
                            value={product?.name ?? ""}
                            onChange={(e) => handleInputChange(e)}
                        />
                    </div>
                    <div className="flex flex-col mb-4">
                        <label
                            htmlFor="description"
                            className="text-sm font-semibold text-gray-600 mb-2"
                        >
                            Product Description
                        </label>
                        <textarea
                            id="description"
                            name="description"
                            className="border-none outline-none py-2 px-3 min-h-[100px] rounded-[4px] bg-gray-100"
                            type="text"
                            placeholder="Enter product description"
                            value={product?.description ?? ""}
                            onChange={(e) => handleInputChange(e)}
                        />
                    </div>
                    <div className="flex flex-col mb-4">
                        <label
                            htmlFor="price"
                            className="text-sm font-semibold text-gray-600 mb-2"
                        >
                            Product Price
                        </label>
                        <input
                            id="price"
                            name="price"
                            className="border-none outline-none py-2 px-3 rounded-[4px] bg-gray-100"
                            type="text"
                            min={0}
                            placeholder="Enter product price"
                            value={product?.price ?? ""}
                            onChange={(e) => handleInputChange(e)}
                        />
                    </div>
                    <div className="flex flex-col mb-4">
                        <label
                            htmlFor="discount"
                            className="text-sm font-semibold text-gray-600 mb-2"
                        >
                            Product Discount
                        </label>
                        <input
                            id="discount"
                            name="discount"
                            className="border-none outline-none py-2 px-3 rounded-[4px] bg-gray-100"
                            type="text"
                            min={0}
                            placeholder="Enter product discount"
                            value={product?.discount ?? ""}
                            onChange={(e) => handleInputChange(e)}
                        />
                    </div>
                    <div className="flex flex-col mb-4">
                        <label
                            htmlFor="stock"
                            className="text-sm font-semibold text-gray-600 mb-2"
                        >
                            Product Stock
                        </label>
                        <input
                            id="stock"
                            name="stock"
                            className="border-none outline-none py-2 px-3 rounded-[4px] bg-gray-100"
                            type="number"
                            min={0}
                            placeholder="Enter product discount"
                            value={product?.stock ?? ""}
                            onChange={(e) => handleInputChange(e)}
                        />
                    </div>
                    <div className="flex flex-col mb-4 cursor-pointer">
                        <label
                            htmlFor="thumbnail"
                            className="text-sm font-semibold text-gray-600 mb-2"
                        >
                            Food Thumbnail
                        </label>
                        <div
                            {...getRootProps()}
                            className="dropzone"
                            style={{
                                border: "2px dashed #ccc",
                                borderRadius: "8px",
                                padding: "16px",
                                textAlign: "center",
                            }}
                        >
                            {product?.thumbnail ? (
                                <img
                                    src={product.thumbnail}
                                    alt="Food Thumbnail"
                                    className="object-cover mx-auto"
                                />
                            ) : (
                                <>
                                    <input {...getInputProps()} />
                                    <p style={{ margin: "8px 0" }}>
                                        Drag and drop an image here, or click to
                                        select an image
                                    </p>
                                    <ImageIcon
                                        style={{
                                            fontSize: "48px",
                                            color: "#555",
                                        }}
                                    />
                                </>
                            )}
                        </div>
                    </div>
                    <div className="flex flex-col mb-4">
                        <label
                            htmlFor="images"
                            className="text-sm font-semibold text-gray-600 mb-2"
                        >
                            Product Images (maximum 5 images)
                        </label>
                        <input
                            type="file"
                            name="images"
                            id="images"
                            accept="image/jpeg, image/jpg, image/png"
                            onChange={(e) => handleProductImage(e)}
                            multiple
                        />
                        {product?.images ? (
                            <div className="border-2 border-dashed rounded-lg p-4 flex flex-wrap items-center gap-4 mt-4">
                                {JSON.parse(product?.images)?.map(
                                    (image, index) => (
                                        <img
                                            key={index}
                                            src={image}
                                            width={100}
                                            height={100}
                                            className="object-cover border-[1px] border-primary rounded-sm"
                                        />
                                    )
                                )}
                            </div>
                        ) : null}
                    </div>
                    <div className="flex justify-between">
                        <Button
                            type="button"
                            onClick={handleClose}
                            className="bg-slate-200 hover:bg-slate-300 px-4 transition-all duration-300 text-black font-medium"
                        >
                            CLOSE
                        </Button>
                        <Button
                            type="submit"
                            autoFocus
                            className="flex justify-center items-center px-4 gap-2 bg-primary-light uppercase hover:bg-primary-dark transition-all duration-300 text-white font-medium"
                        >
                            {editingProductId ? "Update Food" : "Add Food"}{" "}
                            <span
                                className={`bar ${isLoading ? "" : "hidden"}`}
                            />
                        </Button>
                    </div>
                </form>
            </ProductFormDialog>
            <div className="bg-white px-6 py-4 rounded-md">
                <AdminTable
                    rows={rows}
                    columns={columns}
                    pageSizeOptions={[10, 20, 30, 40, 50]}
                />
            </div>
            <Toast />
        </div>
    );
};

export default MenuPage;
